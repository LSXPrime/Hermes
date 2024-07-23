namespace Hermes.Application.Exceptions;

public class NotFoundException(string message) : ApiException(message, 404);