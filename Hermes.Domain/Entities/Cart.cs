namespace Hermes.Domain.Entities;

public class Cart : BaseEntity
{
    public decimal TotalPrice { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? AppliedCouponCode { get; set; }
    public Coupon? AppliedCoupon { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = [];

    // Navigation Properties
    public int UserId { get; set; } 
    public User User { get; set; }
}