namespace Hermes.Application.DTOs;

public class ReviewDto 
{
    public int Id { get; set; } 
    public int Rating { get; set; }
    public string ReviewText { get; set; }
    public int ProductId { get; set; } 
    public int UserId { get; set; } 
}