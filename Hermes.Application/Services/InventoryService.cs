using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Enums;
using Hermes.Domain.Interfaces;

namespace Hermes.Application.Services;

public class InventoryService(IUnitOfWork unitOfWork) : IInventoryService
{
    /// <summary>
    /// Checks if a specific product variant has enough stock available for a given quantity.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to check.</param>
    /// <param name="quantity">The quantity to check against available stock.</param>
    /// <returns>True if enough stock is available, false otherwise.</returns>
    public async Task<bool> IsInStockAsync(int productVariantId, int quantity)
    {
        var inventory = await unitOfWork.Inventories.GetByProductVariantIdAsync(productVariantId);
        return inventory != null && inventory.QuantityOnHand >= quantity;
    }

    /// <summary>
    /// Reserves a specified quantity of stock for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to reserve stock for.</param>
    /// <param name="quantity">The quantity to reserve.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task ReserveStockAsync(int productVariantId, int quantity)
    {
        var inventory = await unitOfWork.Inventories.GetByProductVariantIdAsync(productVariantId);
        if (inventory == null || inventory.QuantityOnHand < quantity)
        {
            throw new OutOfStockException("Insufficient stock available.");
        }

        await unitOfWork.Inventories.UpdateQuantityAsync(inventory.Id, quantity, isReservation: true); 
    }

    /// <summary>
    /// Releases a previously reserved quantity of stock for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to release stock for.</param>
    /// <param name="quantity">The quantity to release.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task ReleaseStockAsync(int productVariantId, int quantity)
    {
        var inventory = await unitOfWork.Inventories.GetByProductVariantIdAsync(productVariantId);
        if (inventory == null)
        {
            return; 
        }

        await unitOfWork.Inventories.UpdateQuantityAsync(inventory.Id, quantity); 
    }

    /// <summary>
    /// Gets the current quantity of stock on hand for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to get the quantity for.</param>
    /// <returns>The quantity of stock on hand for the product variant.</returns>
    public async Task<int> GetQuantityOnHandAsync(int productVariantId)
    {
        var inventory = await unitOfWork.Inventories.GetByProductVariantIdAsync(productVariantId);
        return inventory?.QuantityOnHand ?? 0;
    }

    /// <summary>
    /// Gets the quantity of stock reserved for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to get the reserved quantity for.</param>
    /// <returns>The quantity of stock reserved for the product variant.</returns>
    public async Task<int> GetReservedQuantityAsync(int productVariantId)
    {
        var inventory = await unitOfWork.Inventories.GetByProductVariantIdAsync(productVariantId);
        return inventory?.ReservedQuantity ?? 0;
    }

    /// <summary>
    /// Creates a new inventory record for a product variant with an initial quantity.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to create inventory for.</param>
    /// <param name="initialQuantity">The initial quantity of stock to assign to the variant.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task CreateInventoryForVariantAsync(int productVariantId, int initialQuantity)
    {
        var inventory = new Inventory
        {
            ProductVariantId = productVariantId,
            QuantityOnHand = initialQuantity,
            ReservedQuantity = 0,
            ReorderThreshold = 0,
            IsReorderNeeded = false
        };

        await unitOfWork.Inventories.AddAsync(inventory);
    }


    /// <summary>
    /// Updates the quantity of stock for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to update.</param>
    /// <param name="quantityChange">The new quantity of stock.</param>
    /// <param name="operation">The operation to perform on the quantity. Defaults to Set (replace the current quantity with the new quantity).</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateQuantityAsync(int productVariantId, int quantityChange, Operator operation = Operator.Set)
    {
        var inventory = await unitOfWork.Inventories.GetByProductVariantIdAsync(productVariantId);
        if (inventory == null)
        {
            throw new NotFoundException($"Inventory record not found for ProductVariantId: {productVariantId}");
        }

        inventory.QuantityOnHand = operation switch
        {
            Operator.Add => inventory.QuantityOnHand + quantityChange,
            Operator.Subtract => inventory.QuantityOnHand - quantityChange,
            Operator.Set => inventory.QuantityOnHand,
            _ => inventory.QuantityOnHand
        };

        inventory.IsReorderNeeded = inventory.QuantityOnHand < inventory.ReorderThreshold;
        await unitOfWork.Inventories.UpdateAsync(inventory);
    }
}