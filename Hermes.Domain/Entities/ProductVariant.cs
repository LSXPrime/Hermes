using System.ComponentModel.DataAnnotations;

namespace Hermes.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } 

    public string ImageUrl { get; set; }
    public string SKU { get; set; }

    public decimal PriceAdjustment { get; set; }
    public int Quantity { get; set; }
    public bool InStock { get; set; } 

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    // Navigation properties
    public ICollection<ProductVariantOption> Options { get; set; } = [];
}