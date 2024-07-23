namespace Hermes.Application.DTOs;

public class CreateReviewDto
{
    public int Rating { get; set; }
    public string ReviewText { get; set; }
    public int ProductId { get; set; } 
    public int UserId { get; set; }
}