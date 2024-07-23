using Hermes.API.Middlewares;
using Hermes.Application.Interfaces;
using Hermes.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hermes.API.Attributes;

/// <summary>
/// Attribute for authorizing access to an action.
/// </summary>
/// <param name="allowAnonymous">Indicates whether anonymous access is allowed.</param>
/// <param name="roles">The roles that are authorized to access the action.</param>
public class AuthorizeMiddlewareAttribute(bool allowAnonymous, params string[] roles) : Attribute, IAsyncActionFilter
{
    public AuthorizeMiddlewareAttribute(params string[] roles) : this(false, roles)
    {
    }
    
    public AuthorizeMiddlewareAttribute() : this(true, ["User"])
    {
    }
    

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var authService = httpContext.RequestServices.GetRequiredService<IAuthService>();
        var unitOfWork = httpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var middleware = new AuthorizationMiddleware(async _ => await next(), authService, unitOfWork, roles, allowAnonymous);
        await middleware.InvokeAsync(httpContext);
    }
}