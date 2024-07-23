namespace Hermes.Domain.Entities;

public class ProductVariantOption : BaseEntity
{
    // Navigation properties
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; }

    public string Name { get; set; }
    public string Value { get; set; }
}