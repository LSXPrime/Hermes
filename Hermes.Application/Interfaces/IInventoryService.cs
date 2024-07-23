using Hermes.Domain.Enums;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages Inventory operations.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Checks if a specific product variant has enough stock available for a given quantity.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to check.</param>
    /// <param name="quantity">The quantity to check against available stock.</param>
    /// <returns>True if enough stock is available, false otherwise.</returns>
    Task<bool> IsInStockAsync(int productVariantId, int quantity);

    /// <summary>
    /// Reserves a specified quantity of stock for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to reserve stock for.</param>
    /// <param name="quantity">The quantity to reserve.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task ReserveStockAsync(int productVariantId, int quantity);

    /// <summary>
    /// Releases a previously reserved quantity of stock for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to release stock for.</param>
    /// <param name="quantity">The quantity to release.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task ReleaseStockAsync(int productVariantId, int quantity);

    /// <summary>
    /// Gets the current quantity of stock on hand for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to get the quantity for.</param>
    /// <returns>The quantity of stock on hand for the product variant.</returns>
    Task<int> GetQuantityOnHandAsync(int productVariantId);

    /// <summary>
    /// Gets the quantity of stock reserved for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to get the reserved quantity for.</param>
    /// <returns>The quantity of stock reserved for the product variant.</returns>
    Task<int> GetReservedQuantityAsync(int productVariantId);

    /// <summary>
    /// Creates a new inventory record for a product variant with an initial quantity.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to create inventory for.</param>
    /// <param name="initialQuantity">The initial quantity of stock to assign to the variant.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task CreateInventoryForVariantAsync(int productVariantId, int initialQuantity);

    /// <summary>
    /// Updates the quantity of stock for a product variant.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant to update.</param>
    /// <param name="quantityChange">The new quantity of stock.</param>
    /// <param name="operation">The operation to perform on the quantity. Defaults to Set (replace the current quantity with the new quantity).</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UpdateQuantityAsync(int productVariantId, int quantityChange, Operator operation = Operator.Set);
}