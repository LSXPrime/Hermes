using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Hermes.Infrastructure.Repositories;

public class UnitOfWork(
    HermesDbContext context,
    ICartItemRepository cartItems,
    ICartRepository carts,
    ICategoryRepository categories,
    IOrderRepository orders,
    IOrderHistoryRepository orderHistory,
    IProductRepository products,
    IInventoryRepository inventories,
    IRefreshTokenRepository refreshTokens,
    IReviewRepository reviews,
    ICouponRepository coupons,
    IRoleRepository roles,
    IUserRepository users)
    : IUnitOfWork
{
    public ICartItemRepository CartItems { get; } = cartItems;
    public ICartRepository Carts { get; } = carts;
    public ICategoryRepository Categories { get; } = categories;
    public IOrderRepository Orders { get; } = orders;
    public IOrderHistoryRepository OrderHistory { get; } = orderHistory;
    public IProductRepository Products { get; } = products;
    public IInventoryRepository Inventories { get; } = inventories;
    public IRefreshTokenRepository RefreshTokens { get; } = refreshTokens;
    public IReviewRepository Reviews { get; } = reviews;
    public ICouponRepository Coupons { get; } = coupons;
    public IRoleRepository Roles { get; } = roles;
    public IUserRepository Users { get; } = users;

    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Asynchronously saves changes to the database.
    /// </summary>
    /// <returns>True if changes were saved successfully, false otherwise.</returns>
    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Asynchronously begins a new transaction.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task BeginTransactionAsync()
    {
        _transaction = await context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Asynchronously commits the current transaction.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            _transaction.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Asynchronously rolls back the current transaction.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync(); 
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the UnitOfWork object.
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        context.Dispose();
    }
}