using Hermes.Domain.Entities;

namespace Hermes.Domain.Interfaces;

/// <summary>
/// Defines the repository interface for managing Coupon entities.
/// </summary>
public interface ICouponRepository : IGenericRepository<Coupon>
{
    /// <summary>
    /// Retrieves a coupon from the repository based on the provided coupon code.
    /// </summary>
    /// <param name="code">The coupon code to search for.</param>
    /// <returns>The retrieved Coupon, or null if no matching coupon is found.</returns>
    Task<Coupon?> GetByCodeAsync(string code);

    /// <summary>
    /// Retrieves a collection of active coupons from the repository.
    /// </summary>
    /// <returns>An IEnumerable of Coupon objects representing active coupons.</returns>
    Task<IEnumerable<Coupon>> GetActiveCouponsAsync();

    /// <summary>
    /// Retrieves a collection of expired coupons from the repository.
    /// </summary>
    /// <returns>An IEnumerable of Coupon objects representing expired coupons.</returns>
    Task<IEnumerable<Coupon>> GetExpiredCouponsAsync();
}