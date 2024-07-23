using System.Text.Json;
using EasyCaching.InMemory;
using EFCoreSecondLevelCacheInterceptor;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hermes.API.Filters;
using Hermes.API.Utilities;
using Hermes.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using AutoValidationExtensions = SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions.ServiceCollectionExtensions;

namespace Hermes.API;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 1. Configure Database
        services.AddControllers();
        services.AddDbContext<HermesDbContext>((serviceProvider, options) =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptionsBuilder =>
                    {
                        sqlServerOptionsBuilder
                            .CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds)
                            .EnableRetryOnFailure()
                            .MigrationsAssembly(typeof(HermesDbContext).Assembly.FullName);
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()));

        // 2. Configure Caching
        const string cacheProvider = "InMemory";
        services.AddEFSecondLevelCache(options =>
            options.UseEasyCachingCoreProvider(cacheProvider, isHybridCache: false).ConfigureLogging(true)
                .UseCacheKeyPrefix("EF_")
                .CacheAllQueries(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(30))
                .UseDbCallsIfCachingProviderIsDown(TimeSpan.FromMinutes(1))
        );

        services.AddEasyCaching(options =>
        {
            options.UseInMemory(config =>
            {
                config.DBConfig = new InMemoryCachingOptions
                {
                    ExpirationScanFrequency = 60,
                    SizeLimit = 100,
                    EnableReadDeepClone = false,
                    EnableWriteDeepClone = false,
                };
                config.MaxRdSecond = 120;
                config.EnableLogging = false;
                config.LockMs = 5000;
                config.SleepMs = 300;
            }, cacheProvider);
        });


        // 3. Configure Validators
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<Startup>();
        AutoValidationExtensions.AddFluentValidationAutoValidation(services, options =>
        {
            options.DisableBuiltInModelValidation = true;
            options.EnableBodyBindingSourceAutomaticValidation = true;
            options.EnableFormBindingSourceAutomaticValidation = true;
            options.EnableQueryBindingSourceAutomaticValidation = true;
            options.EnablePathBindingSourceAutomaticValidation = true;
            options.EnableCustomBindingSourceAutomaticValidation = true;
        });
        
        // 4. Configure AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // 5. Configure Filters
        services.AddMvc(options => { options.Filters.Add<ApiExceptionFilter>(); });

        // 6. Configure Settings
        services.AddSettings(configuration);

        // 7. Register Repositories
        services.AddRepositories();

        // 8. Register Services
        services.AddServices();

        // 9. Configure Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => { endpoints.MapControllers(); })
            .UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var error = new { message = contextFeature.Error.Message };
                        await context.Response.WriteAsync(JsonSerializer.Serialize(error));

                        // Logging
                        Console.WriteLine($"Error: {contextFeature.Error}");
                        Console.WriteLine($"Stack Trace: {contextFeature.Error.StackTrace}");
                    }
                });
            });
    }
}