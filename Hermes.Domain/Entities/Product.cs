using System.ComponentModel.DataAnnotations;
using Hermes.Domain.Enums;

namespace Hermes.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public double Weight { get; set; }
    public string? WeightUnit { get; set; }
    public double Height { get; set; }
    public string? HeightUnit { get; set; }
    public double Width { get; set; }
    public string? WidthUnit { get; set; }
    public double Length { get; set; }
    public string? LengthUnit { get; set; }
    public List<string> Tags { get; set; } = [];
    public HostedAt HostedAt { get; set; }

    // Navigation Properties
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int SellerId { get; set; }
    public User Seller { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<ProductVariant> Variants { get; set; } = [];
}