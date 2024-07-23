using Hermes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Data.Context;

/// <summary>
/// Represents the database context for the Hermes application, responsible for managing entities and their relationships.
/// </summary>
public class HermesDbContext(DbContextOptions<HermesDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductVariantOption> ProductVariantOptions { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderHistory> OrderHistory { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Configures the relationships between entities in the database context.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the relationships.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product - Category (One-to-Many)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProductVariant - Product (Many-to-One)
        modelBuilder.Entity<ProductVariant>()
            .HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductVariantOption - ProductVariant (Many-to-One)
        modelBuilder.Entity<ProductVariantOption>()
            .HasOne(pvo => pvo.ProductVariant)
            .WithMany(pv => pv.Options)
            .HasForeignKey(pvo => pvo.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product - Seller (User) Relationship 
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Seller)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        // User - Order (One-to-Many)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // User - Address (One-to-One)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<User>(u => u.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order - Shipping Address (One-to-One)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.ShippingAddress)
            .WithOne()
            .HasForeignKey<Order>(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        // Order - Billing Address (One-to-One)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.BillingAddress)
            .WithOne()
            .HasForeignKey<Order>(o => o.BillingAddressId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Order - OrderItem (One-to-Many)
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderItem - Product (One-to-Many) - To track product at purchase time
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderHistory - Order (Many-to-One)
        modelBuilder.Entity<OrderHistory>()
            .HasOne(oh => oh.Order)
            .WithMany(o => o.OrderHistory)
            .HasForeignKey(oh => oh.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Review - User (Many-to-One) 
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Review - Product (Many-to-One)
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cart - User (One-to-One)
        modelBuilder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithOne(u => u.Cart)
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cart - CartItem (One-to-Many)
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // CartItem - Product (Many-to-One) - For product details in the cart
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany() 
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category - Parent Category (Self-Referencing One-to-Many for Subcategories)
        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(pc => pc.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}