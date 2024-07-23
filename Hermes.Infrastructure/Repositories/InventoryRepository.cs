using Hermes.Application.Exceptions;
using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class InventoryRepository(HermesDbContext context)
    : GenericRepository<Inventory>(context), IInventoryRepository
{
    /// <summary>
    /// Retrieves an Inventory record from the repository based on the provided product variant ID.
    /// </summary>
    /// <param name="productVariantId">The ID of the product variant associated with the Inventory record.</param>
    /// <returns>The retrieved Inventory record, or null if no matching record is found.</returns>
    public async Task<Inventory?> GetByProductVariantIdAsync(int productVariantId)
    {
        return await Context.Inventories.FirstOrDefaultAsync(i => i.ProductVariantId == productVariantId);
    }

    /// <summary>
    /// Updates the quantity of an inventory record based on the provided inventory ID and quantity change.
    /// </summary>
    /// <param name="inventoryId">The ID of the inventory record to update.</param>
    /// <param name="quantityChange">The change in quantity to apply (can be positive or negative).</param>
    /// <param name="isReservation">Indicates whether the quantity change is a reservation (true) or an actual update (false). Defaults to false.</param>
    public async Task UpdateQuantityAsync(int inventoryId, int quantityChange, bool isReservation = false)
    {
        const int maxRetries = 3;
        var retryCount = 0;
        bool updateSuccessful;

        do
        {
            updateSuccessful = await TryUpdateInventoryAsync(inventoryId, quantityChange, isReservation);
            retryCount++;

            if (!updateSuccessful)
                await Task.Delay(100);
        } while (!updateSuccessful && retryCount < maxRetries);

        if (!updateSuccessful)
        {
            throw new OutOfStockException("The inventory has been updated by another process. Please try again.");
        }
    }

    /// <summary>
    /// Tries to update an inventory record based on the provided inventory ID and quantity change.
    /// </summary>
    /// <param name="inventoryId">The ID of the inventory record to update.</param>
    /// <param name="quantityChange">The change in quantity to apply (can be positive or negative).</param>
    /// <param name="isReservation">Indicates whether the quantity change is a reservation (true) or an actual update (false). Defaults to false.</param>
    /// <returns>True if the inventory record was successfully updated, false otherwise.</returns>
    private async Task<bool> TryUpdateInventoryAsync(int inventoryId, int quantityChange, bool isReservation)
    {
        var inventory = await Context.Inventories.FindAsync(inventoryId);
        if (inventory == null)
        {
            throw new NotFoundException("Inventory record not found.");
        }

        EntryPropertyChange(inventory, i => i.RowVersion!, inventory.RowVersion!, true);

        if (isReservation)
        {
            inventory.ReservedQuantity += quantityChange;
            inventory.QuantityOnHand = Math.Max(0, inventory.QuantityOnHand - quantityChange);
        }
        else
        {
            inventory.QuantityOnHand += quantityChange;
            // If it's not a reservation, it means an actual stock change (sale, return, etc.)
            inventory.ReservedQuantity = Math.Max(0, inventory.ReservedQuantity - quantityChange);
        }

        inventory.IsReorderNeeded = inventory.QuantityOnHand < inventory.ReorderThreshold;

        try
        {
            Context.Inventories.Update(inventory);
            await Context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            await Context.Entry(inventory).ReloadAsync();
            return false;
        }
    }
}