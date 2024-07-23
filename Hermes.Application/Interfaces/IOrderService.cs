using Hermes.Application.DTOs;
using Hermes.Domain.Enums;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages Order operations.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Gets a preview of an order based on the provided order details.
    /// </summary>
    /// <param name="orderDto">The CreateOrderDto object containing the order details to preview.</param>
    /// <returns>An OrderPreviewDto object representing the preview of the order, or null if the order cannot be created.</returns>
    Task<OrderPreviewDto?> GetOrderPreviewAsync(CreateOrderDto orderDto);

    /// <summary>
    /// Creates a new order based on the provided order details.
    /// </summary>
    /// <param name="orderDto">The CreateOrderDto object containing the order details to create.</param>
    /// <returns>An OrderDto object representing the newly created order.</returns>
    Task<OrderDto?> CreateOrderAsync(CreateOrderDto orderDto);

    /// <summary>
    /// Retrieves a collection of orders associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose orders to retrieve.</param>
    /// <returns>An IEnumerable of OrderDto objects representing the user's orders.</returns>
    Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId);

    /// <summary>
    /// Retrieves detailed information for a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve details for.</param>
    /// <returns>The retrieved OrderDto object, or null if no matching order is found.</returns>
    Task<OrderDto?> GetOrderByIdAsync(int orderId);

    /// <summary>
    /// Retrieves a paged collection of all orders.
    /// </summary>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved orders and pagination information.</returns>
    Task<PagedResult<OrderDto>> GetAllOrdersAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to update.</param>
    /// <param name="orderDto">The OrderDto object containing the updated order details.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UpdateOrderAsync(int orderId, OrderDto orderDto);

    /// <summary>
    /// Updates the status of an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to update.</param>
    /// <param name="newStatus">The new status to assign to the order.</param>
    /// <param name="notes">Optional notes to be added to the order history.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? notes = null);

    /// <summary>
    /// Cancels an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to cancel.</param>
    /// <returns>True if the order was successfully canceled, false otherwise.</returns>
    Task<bool> CancelOrderAsync(int orderId);

    /// <summary>
    /// Deletes an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to delete.</param>
    /// <returns>True if the order was successfully deleted, false otherwise.</returns>
    Task<bool> DeleteOrderAsync(int orderId);
}