using Hermes.Domain.Entities;
using Hermes.Domain.Enums;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class OrderRepository(HermesDbContext context) : GenericRepository<Order>(context), IOrderRepository
{
    /// <summary>
    /// Retrieves a collection of orders associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose orders to retrieve.</param>
    /// <returns>An IEnumerable of Order objects representing the user's orders.</returns>
    public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
    {
        return await Context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves detailed information for a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve details for.</param>
    /// <returns>The retrieved Order object, or null if no matching order is found.</returns>
    public async Task<Order?> GetOrderDetailsAsync(int orderId)
    {
        return await Context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    /// <summary>
    /// Retrieves a collection of orders based on their status.
    /// </summary>
    /// <param name="status">The order status to filter by.</param>
    /// <returns>An IEnumerable of Order objects matching the specified status.</returns>
    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await Context.Orders
            .Where(o => o.OrderStatus == status)
            .ToListAsync();
    }
}