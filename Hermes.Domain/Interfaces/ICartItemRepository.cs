using Hermes.Domain.Entities;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing CartItem entities.
/// </summary>
public interface ICartItemRepository : IGenericRepository<CartItem>
{
    /// <summary>
    /// Retrieves a CartItem from the repository based on the provided cart ID and product ID.
    /// </summary>
    /// <param name="cartId">The ID of the cart containing the CartItem.</param>
    /// <param name="productId">The ID of the product associated with the CartItem.</param>
    /// <returns>The retrieved CartItem, or null if no matching CartItem is found.</returns>
    Task<CartItem?> GetCartItemAsync(int cartId, int productId); 
}