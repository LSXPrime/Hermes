using Hermes.Application.Interfaces;
using Hermes.Application.Services;
using Hermes.Domain.Interfaces;
using Hermes.Domain.Settings;
using Hermes.Infrastructure.Repositories;
using Hermes.Infrastructure.Services;
using Hermes.Infrastructure.Utilities;

namespace Hermes.API.Utilities;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add repositories to.</param>
    /// <returns>The service collection with added repositories.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICartItemRepository, CartItemRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Adds services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection with added services.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICloudStorageHelper, CloudStorageHelper>();
        services.AddScoped<IImageHelper, ImageHelper>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, StripePaymentService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IShippingService, MultiShippingService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
    
    /// <summary>
    /// Adds settings to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add settings to.</param>
    /// <param name="configuration">The configuration to get settings from.</param>
    /// <returns>The service collection with added settings.</returns>
    public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<AzureStorageSettings>(configuration.GetSection("AzureStorageSettings"));
        services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
        services.Configure<WarehouseAddressSettings>(configuration.GetSection("WarehouseAddress"));
        return services;
    }
}