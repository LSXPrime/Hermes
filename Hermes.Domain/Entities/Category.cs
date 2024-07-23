namespace Hermes.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }

    public int? ParentCategoryId { get; set; }
    public Category ParentCategory { get; set; }

    // Navigation Properties
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<Category> SubCategories { get; set; } = [];
}