namespace Hermes.Application.DTOs;

public class CartItemDto 
{
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }

    public int ProductId { get; set; }
    public int ProductVariantId { get; set; }
}