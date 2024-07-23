using Hermes.Domain.Entities;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing Cart entities.
/// </summary>
public interface ICartRepository : IGenericRepository<Cart>
{
    /// <summary>
    /// Retrieves a Cart from the repository based on the provided user ID.
    /// </summary>
    /// <param name="userId">The ID of the user associated with the Cart.</param>
    /// <returns>The retrieved Cart, or null if no matching Cart is found.</returns>
    Task<Cart?> GetCartByUserIdAsync(int userId);

    /// <summary>
    /// Clears the contents of a cart based on the provided cart ID.
    /// </summary>
    /// <param name="cartId">The ID of the cart to clear.</param>
    /// <returns>True if the cart was successfully cleared, false otherwise.</returns>
    Task<bool> ClearCartAsync(int cartId);
}