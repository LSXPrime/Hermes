namespace Hermes.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }

    // Navigation Properties
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
    
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
}