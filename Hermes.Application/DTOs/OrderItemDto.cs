namespace Hermes.Application.DTOs;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }
}