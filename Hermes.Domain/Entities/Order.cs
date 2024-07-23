using Hermes.Domain.Enums;

namespace Hermes.Domain.Entities;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }

    public string Currency { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DiscountedAmount { get; set; }
    public string? AppliedCouponCode { get; set; }

    public string? PaymentIntentId { get; set; }
    public string? CheckoutSessionId { get; set; }

    // Navigation Properties
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<OrderHistory> OrderHistory { get; set; } = [];
    public int ShippingAddressId { get; set; }
    public int BillingAddressId { get; set; }
}