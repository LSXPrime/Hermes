using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

public class ControllerBaseEx : ControllerBase
{
    /// <summary>
    /// Gets the current user's ID.
    /// </summary>
    /// <returns>
    /// Returns the current user's ID.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user ID is not found in the claims.</exception>
    protected int CurrentUserId
    {
        get
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (claim == null || !int.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("User ID not found in claims.");

            return userId;
        }
    }
    
    /// <summary>
    /// Gets the current user's role.
    /// </summary>
    /// <returns>
    /// Returns the current user's role.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user role is not found in the claims.</exception>
    protected string CurrentUserRole
    {
        get
        {
            var claim = User.FindFirstValue(ClaimTypes.Role);
            if (claim == null)
                throw new UnauthorizedAccessException("User Role not found in claims.");

            return claim;
        }
    }
}