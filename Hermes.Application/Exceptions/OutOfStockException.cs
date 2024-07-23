namespace Hermes.Application.Exceptions;

public class OutOfStockException(string message) : ApiException(message, 409)
{
    
}