namespace Hermes.Application.DTOs;

public class CartDto 
{
    public int Id { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public string? AppliedCouponCode { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
}