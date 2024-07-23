using Hermes.Domain.Entities;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing OrderHistory entities.
/// </summary>
public interface IOrderHistoryRepository : IGenericRepository<OrderHistory>
{
    /// <summary>
    /// Retrieves a collection of OrderHistory records associated with a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve the history for.</param>
    /// <returns>An IEnumerable of OrderHistory objects representing the order history.</returns>
    Task<IEnumerable<OrderHistory>> GetHistoryForOrderAsync(int orderId);
}