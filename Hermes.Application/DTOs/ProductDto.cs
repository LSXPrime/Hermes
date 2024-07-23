namespace Hermes.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int SellerId { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<ProductVariantDto> Variants { get; set; } = [];
    public List<ReviewDto> Reviews { get; set; } = [];
}