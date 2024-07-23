namespace Hermes.Application.DTOs;

public class CartCheckoutItemDto
{
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}