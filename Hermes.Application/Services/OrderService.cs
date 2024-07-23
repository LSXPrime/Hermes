using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Enums;
using Hermes.Domain.Interfaces;
using Hermes.Domain.Settings;
using Microsoft.Extensions.Options;

namespace Hermes.Application.Services;

public class OrderService(
    IUnitOfWork unitOfWork,
    IInventoryService inventoryService,
    ICartService cartService,
    IShippingService shippingService,
    IPaymentService paymentService,
    IEmailService emailService,
    IOptions<WarehouseAddressSettings> warehouseAddressSettings,
    IMapper mapper) : IOrderService
{
    /// <summary>
    /// Gets a preview of an order based on the provided order details.
    /// </summary>
    /// <param name="orderDto">The CreateOrderDto object containing the order details to preview.</param>
    /// <returns>An OrderPreviewDto object representing the preview of the order, or null if the order cannot be created.</returns>
    public async Task<OrderPreviewDto?> GetOrderPreviewAsync(CreateOrderDto orderDto)
    {
        var productIds = orderDto.OrderItems.Select(oi => oi.ProductId).ToList();
        var products = (await unitOfWork.Products.GetByIdsAsync(productIds)).ToDictionary(p => p.Id, p => p);

        var shippingRateRequests = new List<ShippingRateRequest>();
        foreach (var orderItemDto in orderDto.OrderItems)
        {
            if (!products.TryGetValue(orderItemDto.ProductId, out var product))
                throw new NotFoundException($"Product with ID {orderItemDto.ProductId} not found.");

            if (!await inventoryService.IsInStockAsync(orderItemDto.ProductId, orderItemDto.Quantity))
                throw new BadRequestException(
                    $"Product '{product.Name}' is out of stock or insufficient quantity available.");

            shippingRateRequests.Add(new ShippingRateRequest
            {
                OriginPostalCode = product.HostedAt == HostedAt.Store
                    ? product.Seller.Address.PostalCode
                    : GetWarehouseAddress().PostalCode,
                DestinationPostalCode = orderDto.ShippingAddress.PostalCode,
                Weight = product.Weight,
                Width = product.Width,
                Height = product.Height,
                Length = product.Length
            });
        }

        var availableShippingRates = await shippingService.GetShippingRatesAsync(shippingRateRequests);
        var totalAmount = CalculateTotalAmount(orderDto.OrderItems);

        return new OrderPreviewDto
        {
            OrderDate = DateTime.UtcNow,
            OrderStatus = OrderStatus.Pending,
            ShippingAddress = orderDto.ShippingAddress,
            BillingAddress = orderDto.BillingAddress,
            OrderItems = orderDto.OrderItems,
            TotalAmount = totalAmount,
            UserId = orderDto.UserId,
            AvailableShippingRates = availableShippingRates
        };
    }

    /// <summary>
    /// Creates a new order based on the provided order details.
    /// </summary>
    /// <param name="orderDto">The CreateOrderDto object containing the order details to create.</param>
    /// <returns>An OrderDto object representing the newly created order.</returns>
    public async Task<OrderDto?> CreateOrderAsync(CreateOrderDto orderDto)
    {
        var productIds = orderDto.OrderItems.Select(oi => oi.ProductId).ToList();
        var products = (await unitOfWork.Products.GetByIdsAsync(productIds)).ToDictionary(p => p.Id, p => p);

        await unitOfWork.BeginTransactionAsync();
        try
        {
            var shippingRateRequests = new List<ShippingRateRequest>();
            foreach (var orderItemDto in orderDto.OrderItems)
            {
                if (!products.TryGetValue(orderItemDto.ProductId, out var product))
                    throw new NotFoundException($"Product with ID {orderItemDto.ProductId} not found.");

                if (!await inventoryService.IsInStockAsync(orderItemDto.ProductId, orderItemDto.Quantity))
                    throw new BadRequestException(
                        $"Product '{product.Name}' is out of stock or insufficient quantity available.");

                await inventoryService.ReserveStockAsync(product.Id, orderItemDto.Quantity);

                shippingRateRequests.Add(new ShippingRateRequest
                {
                    OriginPostalCode = product.HostedAt == HostedAt.Store
                        ? product.Seller.Address.PostalCode
                        : GetWarehouseAddress().PostalCode,
                    DestinationPostalCode = orderDto.ShippingAddress.PostalCode,
                    Weight = product.Weight,
                    Width = product.Width,
                    Height = product.Height,
                    Length = product.Length
                });
            }

            var selectedShippingRate = (await shippingService.GetShippingRatesAsync(shippingRateRequests))
                .FirstOrDefault(rate => rate.Carrier == orderDto.ShippingMethod);

            if (selectedShippingRate == null)
                throw new BadRequestException("Unable to calculate shipping rates.");

            var cart = await cartService.GetCartByUserIdAsync(orderDto.UserId);
            if (cart == null)
            {
                throw new NotFoundException($"Cart not found for user with ID {orderDto.UserId}.");
            }

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.Pending,
                ShippingAddress = mapper.Map<Address>(orderDto.ShippingAddress),
                BillingAddress = mapper.Map<Address>(orderDto.BillingAddress),
                TotalAmount = cart.TotalPrice + selectedShippingRate.TotalRate,
                UserId = orderDto.UserId,
                Currency = orderDto.Currency
            };

            foreach (var orderItemDto in orderDto.OrderItems)
            {
                if (!products.TryGetValue(orderItemDto.ProductId, out var product)) continue;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = orderItemDto.ProductId,
                    ProductVariantId = orderItemDto.ProductVariantId,
                    Quantity = orderItemDto.Quantity,
                    PriceAtPurchase = orderItemDto.PriceAtPurchase
                });
                await inventoryService.UpdateQuantityAsync(product.Id, orderItemDto.Quantity, Operator.Subtract);
            }

            await unitOfWork.Orders.AddAsync(order);
            var orderHistory = new OrderHistory
            {
                OrderId = order.Id,
                PreviousStatus = OrderStatus.Pending,
                NewStatus = OrderStatus.Pending,
                Notes = "Order created."
            };

            await unitOfWork.OrderHistory.AddAsync(orderHistory);

            await unitOfWork.SaveChangesAsync();

            await unitOfWork.Carts.ClearCartAsync(cart.Id);

            await unitOfWork.CommitTransactionAsync();

            var returnedOrder = mapper.Map<OrderDto>(order);
            await emailService.SendOrderConfirmationEmailAsync(returnedOrder);
            return returnedOrder;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            foreach (var orderItemDto in orderDto.OrderItems)
            {
                if (products.TryGetValue(orderItemDto.ProductId, out var product))
                {
                    await inventoryService.ReleaseStockAsync(product.Id, orderItemDto.Quantity);
                }
            }

            throw new BadRequestException(
                $"An error occurred while processing your order. {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a collection of orders associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose orders to retrieve.</param>
    /// <returns>An IEnumerable of OrderDto objects representing the user's orders.</returns>
    public async Task<IEnumerable<OrderDto>> GetOrdersByUserAsync(int userId)
    {
        var orders = await unitOfWork.Orders.GetOrdersByUserAsync(userId);
        return mapper.Map<IEnumerable<OrderDto>>(orders);
    }

    /// <summary>
    /// Retrieves detailed information for a specific order.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve details for.</param>
    /// <returns>The retrieved OrderDto object, or null if no matching order is found.</returns>
    public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
    {
        var order = await unitOfWork.Orders.GetOrderDetailsAsync(orderId);
        if (order == null)
            throw new NotFoundException($"Order with ID {orderId} not found.");

        return mapper.Map<OrderDto>(order);
    }

    /// <summary>
    /// Retrieves a paged collection of all orders.
    /// </summary>
    /// <param name="page">The page number to retrieve. Defaults to 1.</param>
    /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
    /// <returns>A PagedResult object containing the retrieved orders and pagination information.</returns>
    public async Task<PagedResult<OrderDto>> GetAllOrdersAsync(int page = 1, int pageSize = 10)
    {
        var query = unitOfWork.Orders.GetAllAsync();
        query = query.Skip((page - 1) * pageSize).Take(pageSize);
        var orders = await unitOfWork.Orders.ExecuteQueryAsync(query);
        var totalCount = query.Count();

        return new PagedResult<OrderDto>
        {
            Items = mapper.Map<IEnumerable<OrderDto>>(orders),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Updates an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to update.</param>
    /// <param name="orderDto">The OrderDto object containing the updated order details.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateOrderAsync(int orderId, OrderDto orderDto)
    {
        if (orderDto.OrderItems.Count == 0)
        {
            throw new BadRequestException("Order must contain at least one item.");
        }
        
        var order = await unitOfWork.Orders.GetOrderDetailsAsync(orderId);
        if (order == null)
            throw new NotFoundException($"Order with ID {orderId} not found.");
        
        orderDto.UserId = order.UserId;
        order = mapper.Map(orderDto, order);
        await unitOfWork.Orders.UpdateAsync(order);
    }

    /// <summary>
    /// Updates the status of an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to update.</param>
    /// <param name="newStatus">The new status to assign to the order.</param>
    /// <param name="notes">Optional notes to be added to the order history.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? notes = null)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            var order = await unitOfWork.Orders.GetOrderDetailsAsync(orderId);
            if (order == null)
                throw new NotFoundException($"Order with ID {orderId} not found.");

            if (!IsValidStatusTransition(order.OrderStatus, newStatus))
                throw new BadRequestException(
                    $"Invalid order status transition from {order.OrderStatus} to {newStatus}.");

            var orderHistory = new OrderHistory
            {
                OrderId = order.Id,
                PreviousStatus = order.OrderStatus,
                NewStatus = newStatus,
                Notes = notes
            };
            await unitOfWork.OrderHistory.AddAsync(orderHistory);

            order.OrderStatus = newStatus;
            await unitOfWork.Orders.UpdateAsync(order);

            await unitOfWork.CommitTransactionAsync();
            
            if (newStatus is OrderStatus.Shipped or OrderStatus.Delivered)
            {
                await emailService.SendShippingUpdateEmailAsync(mapper.Map<OrderDto>(order), newStatus.ToString()); 
            }
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw new BadRequestException(
                $"An error occurred while updating order status for order with ID {orderId}. {ex.Message}");
        }
    }
    
    /// <summary>
    /// Cancels an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to cancel.</param>
    /// <returns>True if the order was successfully canceled, false otherwise.</returns>
    public async Task<bool> CancelOrderAsync(int orderId)
    {
        await unitOfWork.BeginTransactionAsync();
        try
        {
            var order = await unitOfWork.Orders.GetOrderDetailsAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.");
            }

            if (order.OrderStatus is OrderStatus.Delivered or OrderStatus.Cancelled)
            {
                throw new BadRequestException(
                    $"Order cannot be cancelled in its current status ({order.OrderStatus}).");
            }

            var orderHistory = new OrderHistory
            {
                OrderId = order.Id,
                PreviousStatus = order.OrderStatus,
                NewStatus = OrderStatus.Cancelled,
                Notes = "Order cancelled."
            };
            await unitOfWork.OrderHistory.AddAsync(orderHistory);

            order.OrderStatus = OrderStatus.Cancelled;
            await unitOfWork.Orders.UpdateAsync(order);

            foreach (var orderItem in order.OrderItems)
            {
                await inventoryService.ReleaseStockAsync(orderItem.ProductId, orderItem.Quantity);
            }

            if (order.OrderStatus is OrderStatus.Pending or OrderStatus.Paid)
            {
                var refundDto = new CreateRefundDto
                {
                    OrderId = order.Id,
                    PaymentIntentId = order.PaymentIntentId,
                    Amount = order.TotalAmount,
                };

                var refund = await paymentService.CreateRefundAsync(refundDto);
                if (refund == null)
                    throw new PaymentException($"Failed to create refund for order with ID {orderId} with amount {order.TotalAmount} due to payment error.");
            }

            await unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch (Exception)
        {
            await unitOfWork.RollbackTransactionAsync();
            throw new BadRequestException($"An error occurred while cancelling order with ID {orderId}.");
        }
    }

    /// <summary>
    /// Deletes an existing order.
    /// </summary>
    /// <param name="orderId">The ID of the order to delete.</param>
    /// <returns>True if the order was successfully deleted, false otherwise.</returns>
    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        var order = await unitOfWork.Orders.GetOrderDetailsAsync(orderId);
        if (order == null)
            throw new NotFoundException($"Order with ID {orderId} not found.");

        await unitOfWork.Orders.DeleteAsync(order);
        return true;
    }
    
    /// <summary>
    /// Calculates the total amount of an order
    /// </summary>
    /// <param name="orderItems">List of order items</param>
    /// <returns></returns>
    private decimal CalculateTotalAmount(List<OrderItemDto> orderItems)
    {
        decimal total = 0;

        foreach (var item in orderItems)
        {
            total += item.Quantity * item.PriceAtPurchase;
        }

        return total;
    }

    /// <summary>
    /// Checks if an order status transition is valid
    /// </summary>
    /// <param name="currentStatus">Current order status</param>
    /// <param name="newStatus">New order status</param>
    /// <returns></returns>
    private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending => newStatus is OrderStatus.Paid or OrderStatus.Cancelled,
            OrderStatus.Paid => newStatus is OrderStatus.Processing or OrderStatus.Cancelled,
            OrderStatus.Processing => newStatus is OrderStatus.Shipped or OrderStatus.Cancelled,
            OrderStatus.Shipped => newStatus == OrderStatus.Delivered,
            _ => false
        };
    }
    
    /// <summary>
    /// Gets the warehouse address
    /// </summary>
    /// <returns></returns>
    private Address GetWarehouseAddress()
    {
        return new Address
        {
            Street = warehouseAddressSettings.Value.Street,
            City = warehouseAddressSettings.Value.City,
            State =warehouseAddressSettings.Value.State,
            Country = warehouseAddressSettings.Value.Country,
            PostalCode = warehouseAddressSettings.Value.PostalCode
        };
    }
}