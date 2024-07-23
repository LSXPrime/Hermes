using Hermes.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hermes.API.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// Handles the exception
    /// </summary>
    /// <param name="context">The context of the exception</param>
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ApiException apiException)
        {
            context.HttpContext.Response.StatusCode = apiException.StatusCode;

            var problemDetails = new ProblemDetails
            {
                Title = apiException.Message,
                Status = apiException.StatusCode
            };

            // Return the problem details as the response
            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = apiException.StatusCode,
                Value = apiException.Message
            };
        }
    }
}