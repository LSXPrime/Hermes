namespace Hermes.Application.DTOs;

public class ProductVariantDto 
{
    public int Id { get; set; }
    public string SKU { get; set; } 
    public string ImageUrl { get; set; }
    public decimal PriceAdjustment { get; set; } 
    public int Quantity { get; set; } 
    public bool InStock { get; set; } 
    public List<ProductVariantOptionDto> Options { get; set; } = [];
}