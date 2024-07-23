namespace Hermes.Application.DTOs;

public class CreateProductVariantDto 
{
    public string SKU { get; set; } 
    
    public string ImageUrl { get; set; }
    
    public decimal Price { get; set; } 

    public int Quantity { get; set; } 

    public bool InStock { get; set; } 

    public List<ProductVariantOptionDto> Options { get; set; } = []; 
}