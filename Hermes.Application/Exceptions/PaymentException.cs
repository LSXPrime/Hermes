namespace Hermes.Application.Exceptions;

public class PaymentException(string message, Exception? innerException = null) : ApiException($"Payment error: {message}. exception: {innerException}", 500)
{
    
}