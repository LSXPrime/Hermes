using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Hermes.Infrastructure.Repositories;

public class CouponRepository(HermesDbContext context) :
    GenericRepository<Coupon>(context), ICouponRepository
{
    /// <summary>
    /// Retrieves a coupon from the repository based on the provided coupon code.
    /// </summary>
    /// <param name="code">The coupon code to search for.</param>
    /// <returns>The retrieved Coupon, or null if no matching coupon is found.</returns>
    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        return await Context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
    }

    /// <summary>
    /// Retrieves a collection of active coupons from the repository.
    /// </summary>
    /// <returns>An IEnumerable of Coupon objects representing active coupons.</returns>
    public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
    {
        var now = DateTime.UtcNow;
        return await Context.Coupons
            .Where(c => c.StartDate <= now && c.EndDate >= now)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a collection of expired coupons from the repository.
    /// </summary>
    /// <returns>An IEnumerable of Coupon objects representing expired coupons.</returns>
    public async Task<IEnumerable<Coupon>> GetExpiredCouponsAsync()
    {
        var now = DateTime.UtcNow;
        return await Context.Coupons
            .Where(c => c.EndDate < now)
            .ToListAsync();
    }
}