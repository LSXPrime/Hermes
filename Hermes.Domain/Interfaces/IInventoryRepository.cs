using Hermes.Domain.Entities;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing Inventory entities.
/// </summary>
public interface IInventoryRepository : IGenericRepository<Inventory>
{
    /// <summary>
    /// Retrieves an Inventory record from the repository based on the provided product variant ID.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant associated with the Inventory record.</param>
    /// <returns>The retrieved Inventory record, or null if no matching record is found.</returns>
    Task<Inventory?> GetByProductVariantIdAsync(int productVariantId);

    /// <summary>
    /// Updates the quantity of an inventory record based on the provided inventory ID and quantity change.
    /// </summary>
    /// <param name="inventoryId">The ID of the inventory record to update.</param>
    /// <param name="quantityChange">The change in quantity to apply (can be positive or negative).</param>
    /// <param name="isReservation">Indicates whether the quantity change is a reservation (true) or an actual update (false). Defaults to false.</param>
    Task UpdateQuantityAsync(int inventoryId, int quantityChange, bool isReservation = false); 
}