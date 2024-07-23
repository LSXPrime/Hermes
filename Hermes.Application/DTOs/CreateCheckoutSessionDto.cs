namespace Hermes.Application.DTOs;

public class CreateCheckoutSessionDto
{
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string Currency { get; set; } 
    public string PaymentMethod { get; set; }
    public List<CartCheckoutItemDto> CartItems { get; set; }
}