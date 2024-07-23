namespace Hermes.Application.DTOs;

public class CreateRefundDto
{
    public int OrderId { get; set; }
    public string PaymentIntentId { get; set; }
    public decimal Amount { get; set; }
}