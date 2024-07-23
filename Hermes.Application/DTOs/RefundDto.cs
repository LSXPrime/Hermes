namespace Hermes.Application.DTOs;

public class RefundDto 
{
    public string Id { get; set; }
    public string PaymentIntentId { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
}