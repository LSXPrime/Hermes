using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class CartItemRepository(HermesDbContext context) : GenericRepository<CartItem>(context), ICartItemRepository
{
    /// <summary>
    /// Retrieves a CartItem from the repository based on the provided cart ID and product ID.
    /// </summary>
    /// <param name="cartId">The ID of the cart containing the CartItem.</param>
    /// <param name="productId">The ID of the product associated with the CartItem.</param>
    /// <returns>The retrieved CartItem, or null if no matching CartItem is found.</returns>
    public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
    {
        return await Context.CartItems
            .Include(x => x.Product)
            .ThenInclude(x => x.Seller)
            .ThenInclude(x => x.Address)
            .Include(x => x.ProductVariant)
            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
    }
}