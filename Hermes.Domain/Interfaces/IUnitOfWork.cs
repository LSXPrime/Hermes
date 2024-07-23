namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the interface for a Unit of Work pattern implementation, responsible for managing multiple repositories within a single transaction scope.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    ICartItemRepository CartItems { get; }
    ICartRepository Carts { get; }
    ICategoryRepository Categories { get; }
    IOrderRepository Orders { get; }
    IOrderHistoryRepository OrderHistory { get; }
    IProductRepository Products { get; }
    IInventoryRepository Inventories { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IReviewRepository Reviews { get; }
    ICouponRepository Coupons { get; }
    IRoleRepository Roles { get; }
    IUserRepository Users { get; }

    /// <summary>
    /// Asynchronously saves changes to the database.
    /// </summary>
    /// <returns>True if changes were saved successfully, false otherwise.</returns>
    Task<bool> SaveChangesAsync();

    /// <summary>
    /// Asynchronously begins a new transaction.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Asynchronously commits the current transaction.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Asynchronously rolls back the current transaction.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync();
}