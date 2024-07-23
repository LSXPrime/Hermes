namespace Hermes.Domain.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; } 
    public string ReviewText { get; set; }

    // Navigation Properties
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }
}