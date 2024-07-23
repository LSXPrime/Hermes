namespace Hermes.Application.DTOs;

public class CreatePaymentIntentDto
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } 
    public string PaymentMethod { get; set; }
}