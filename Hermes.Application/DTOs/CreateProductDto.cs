namespace Hermes.Application.DTOs;

public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public double Weight { get; set; }
    public string WeightUnit { get; set; }
    public double Height { get; set; }
    public string HeightUnit { get; set; }
    public double Width { get; set; }
    public string WidthUnit { get; set; }
    public double Length { get; set; }
    public string LengthUnit { get; set; }
    public int CategoryId { get; set; }
    public int SellerId { get; set; }
    public IEnumerable<ProductVariantDto> Variants { get; set; } = []; 
}