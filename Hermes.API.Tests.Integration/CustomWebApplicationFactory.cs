using Hermes.API.Utilities;
using Hermes.Application.Interfaces;
using Hermes.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hermes.Infrastructure.Data.Context;
using Hermes.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Hermes.API.Tests.Integration;

public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
        });
        
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContextOptions
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<HermesDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add an in-memory database for testing
            services.AddDbContext<HermesDbContext>(options =>
            {
                options.UseInMemoryDatabase("HermesTestDb");
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
            
            // Seed data
            services.AddScoped(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                services.AddSettings(configuration);
                var context = sp.GetRequiredService<HermesDbContext>();
                return new DataSeeder(context);
            });
        });
        
        

        builder.UseEnvironment("Testing");
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Seed database on host startup
        using var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        seeder.SeedAsync().Wait();

        return host;
    }
}
