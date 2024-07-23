using Hermes.Application.DTOs;
using Hermes.Domain.Entities;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages Coupon operations.
/// </summary>
public interface ICouponService
{
    /// <summary>
    /// Retrieves a specific coupon by its ID.
    /// </summary>
    /// <param name="id">The ID of the coupon to retrieve.</param>
    /// <returns>A CouponDto object representing the specified coupon, or null if no coupon with the given ID is found.</returns>
    Task<CouponDto?> GetCouponByIdAsync(int id);

    /// <summary>
    /// Applies a coupon to a cart.
    /// </summary>
    /// <param name="cartId">The ID of the cart to apply the coupon to.</param>
    /// <param name="couponCode">The code of the coupon to apply.</param>
    /// <returns>A CartDto object representing the cart after applying the coupon, or null if the coupon is invalid or cannot be applied.</returns>
    Task<CartDto?> ApplyCouponAsync(int cartId, string couponCode);

    /// <summary>
    /// Removes a coupon from a cart.
    /// </summary>
    /// <param name="cartId">The ID of the cart to remove the coupon from.</param>
    /// <returns>A CartDto object representing the cart after removing the coupon.</returns>
    Task<CartDto?> RemoveCouponAsync(int cartId);

    /// <summary>
    /// Checks if a coupon is valid and can be applied to a cart.
    /// </summary>
    /// <param name="couponCode">The code of the coupon to check.</param>
    /// <param name="cartTotal">The total amount of the cart to check the coupon against.</param>
    /// <returns>True if the coupon is valid and can be applied, false otherwise.</returns>
    Task<bool> IsCouponValidAsync(string couponCode, decimal cartTotal);

    /// <summary>
    /// Creates a new coupon.
    /// </summary>
    /// <param name="couponDto">The CreateCouponDto object containing the coupon data to create.</param>
    /// <returns>A CouponDto object representing the newly created coupon.</returns>
    Task<CouponDto?> CreateCouponAsync(CreateCouponDto couponDto);

    /// <summary>
    /// Updates an existing coupon.
    /// </summary>
    /// <param name="couponId">The ID of the coupon to update.</param>
    /// <param name="couponDto">The UpdateCouponDto object containing the updated coupon data.</param>
    /// <returns>A CouponDto object representing the updated coupon.</returns>
    Task<CouponDto?> UpdateCouponAsync(int couponId, UpdateCouponDto couponDto);

    /// <summary>
    /// Deletes an existing coupon.
    /// </summary>
    /// <param name="couponId">The ID of the coupon to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task DeleteCouponAsync(int couponId);

    /// <summary>
    /// Retrieves a collection of active coupons.
    /// </summary>
    /// <returns>An IEnumerable of CouponDto objects representing active coupons.</returns>
    Task<IEnumerable<CouponDto>> GetActiveCouponsAsync();

    /// <summary>
    /// Retrieves a collection of expired coupons.
    /// </summary>
    /// <returns>An IEnumerable of CouponDto objects representing expired coupons.</returns>
    Task<IEnumerable<CouponDto>> GetExpiredCouponsAsync();
}