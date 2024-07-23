using Hermes.Application.DTOs;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages Cart operations.
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Retrieves the cart associated with the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to retrieve.</param>
    /// <returns>A CartDto object representing the user's cart, or null if no cart is found.</returns>
    Task<CartDto?> GetCartByUserIdAsync(int userId);

    /// <summary>
    /// Adds an item to the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <param name="productVariantId">The ID of the product variant to add to the cart.</param>
    /// <param name="quantity">The quantity of the product variant to add.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task AddItemToCartAsync(int userId, int productVariantId, int quantity);

    /// <summary>
    /// Updates the quantity of an item in the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <param name="productId">The ID of the product whose quantity to update.</param>
    /// <param name="newQuantity">The new quantity for the product.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task UpdateCartItemQuantityAsync(int userId, int productId, int newQuantity);

    /// <summary>
    /// Removes an item from the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <param name="productId">The ID of the product to remove from the cart.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task RemoveItemFromCartAsync(int userId, int productId);

    /// <summary>
    /// Clears all items from the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to clear.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task ClearCartAsync(int userId);

    /// <summary>
    /// Applies a coupon to the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <param name="couponCode">The code of the coupon to apply.</param>
    /// <returns>A CartDto object representing the user's cart after applying the coupon, or null if the coupon is invalid or cannot be applied.</returns>
    Task<CartDto?> ApplyCouponAsync(int userId, string couponCode);

    /// <summary>
    /// Removes any applied coupon from the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <returns>A CartDto object representing the user's cart after removing the coupon.</returns>
    Task<CartDto?> RemoveCouponAsync(int userId);
}