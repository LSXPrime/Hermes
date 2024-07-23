using Hermes.Domain.Entities;
using Hermes.Domain.Enums;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing Order entities.
/// </summary>
public interface IOrderRepository : IGenericRepository<Order>
{
    /// <summary>
    /// Retrieves a collection of orders associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose orders to retrieve.</param>
    /// <returns>An IEnumerable of Order objects representing the user's orders.</returns>
    Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);

    /// <summary>
    /// Retrieves detailed information for a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve details for.</param>
    /// <returns>The retrieved Order object, or null if no matching order is found.</returns>
    Task<Order?> GetOrderDetailsAsync(int orderId);

    /// <summary>
    /// Retrieves a collection of orders based on their status.
    /// </summary>
    /// <param name="status">The order status to filter by.</param>
    /// <returns>An IEnumerable of Order objects matching the specified status.</returns>
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
}