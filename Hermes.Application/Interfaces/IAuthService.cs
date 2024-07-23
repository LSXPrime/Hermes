using System.Security.Claims;
using Hermes.Application.DTOs;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that manages user authentication.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token if successful.
    /// </summary>
    /// <param name="login">The login credentials.</param>
    /// <returns>The JWT token DTO containing the access token, and expiration time, or null if authentication failed.</returns>
    Task<JwtTokenDto?> LoginUserAsync(LoginDto login);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="newUser">The new user registration details.</param>
    /// <returns>True if registration was successful, false otherwise.</returns>
    Task<bool> RegisterAsync(RegisterDto newUser);

    /// <summary>
    /// Resets the user's password using a password reset token.
    /// </summary>
    /// <param name="token">The password reset token.</param>
    /// <param name="newPassword">The new password.</param>
    /// <returns>True if the password was reset successfully, false otherwise.</returns>
    Task<bool> ResetPasswordAsync(string token, string newPassword);

    /// <summary>
    /// Sends a password reset email to the specified email address.
    /// </summary>
    /// <param name="email">The email address to send the password reset email to.</param>
    /// <returns>True if the email was sent successfully, false otherwise.</returns>
    Task<bool> RequestPasswordResetAsync(string email);

    /// <summary>
    /// Generates a JWT token for a given user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="userName">The username of the user.</param>
    /// <param name="role">The role of the user.</param>
    /// <returns>The JWT token DTO containing the access token, and expiration times.</returns>
    JwtTokenDto GenerateJwtToken(int userId, string userName, string role);

    /// <summary>
    /// Validates a JWT token and returns the associated ClaimsPrincipal if valid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The ClaimsPrincipal associated with the token, or null if the token is invalid.</returns>
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
}