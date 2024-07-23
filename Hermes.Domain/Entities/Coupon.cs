using Hermes.Domain.Enums;

namespace Hermes.Domain.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; }

    public string? Description { get; set; }

    public CouponType CouponType { get; set; }
    public decimal DiscountAmount { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int? MinimumOrderAmount { get; set; }

    public bool IsActive { get; set; }
}