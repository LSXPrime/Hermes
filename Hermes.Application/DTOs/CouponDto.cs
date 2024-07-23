using Hermes.Domain.Enums;

namespace Hermes.Application.DTOs;

public class CouponDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }
    public CouponType CouponType { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public bool IsActive { get; set; }
}