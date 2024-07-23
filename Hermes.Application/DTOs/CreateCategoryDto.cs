namespace Hermes.Application.DTOs;

public class CreateCategoryDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentCategoryId { get; set; }
}