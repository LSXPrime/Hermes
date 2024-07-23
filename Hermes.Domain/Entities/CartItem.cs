namespace Hermes.Domain.Entities;

public class CartItem : BaseEntity
{
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }

    // Navigation Properties
    public int CartId { get; set; }
    public Cart Cart { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
    
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }
}