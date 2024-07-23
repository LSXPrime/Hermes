using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class CartRepository(HermesDbContext context) : GenericRepository<Cart>(context), ICartRepository
{
    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>
    /// The entity with the specified ID, or null if no such entity exists.
    /// </returns>
    public new async Task<Cart?> GetByIdAsync(int id)
    {
        return await Context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Seller)
            .ThenInclude(x => x.Address)
            .Include(c => c.AppliedCoupon)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Retrieves a Cart from the repository based on the provided user ID.
    /// </summary>
    /// <param name="userId">The ID of the user associated with the Cart.</param>
    /// <returns>The retrieved Cart, or null if no matching Cart is found.</returns>
    public async Task<Cart?> GetCartByUserIdAsync(int userId)
    {
        return await Context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Seller)
            .ThenInclude(x => x.Address)
            .Include(c => c.AppliedCoupon)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    /// <summary>
    /// Clears the contents of a cart based on the provided cart ID.
    /// </summary>
    /// <param name="cartId">The ID of the cart to clear.</param>
    /// <returns>True if the cart was successfully cleared, false otherwise.</returns>
    public async Task<bool> ClearCartAsync(int cartId)
    {
        var cartItems = Context.CartItems.Where(ci => ci.CartId == cartId);
        Context.CartItems.RemoveRange(cartItems);
        await Context.SaveChangesAsync();
        return true;
    }
}