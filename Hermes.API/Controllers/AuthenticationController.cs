using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Hermes.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IAuthService authService, IUnitOfWork unitOfWork) : ControllerBase
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="newUser">The user registration details.</param>
    /// <returns>
    /// Returns an OK response with a message "User registered successfully." if the registration is successful.
    /// Returns a BadRequest response with a message "Username or email is already taken." if the username or email is already taken.
    /// Returns a BadRequest response with the ModelState if the request is invalid.
    /// </returns>
    [HttpPost("register")] 
    public async Task<IActionResult> Register([FromBody] RegisterDto newUser)
    {
        var result = await authService.RegisterAsync(newUser);
        return result ? Ok("User registered successfully.") : BadRequest("Username or email is already taken.");
    }

    /// <summary>
    /// Logs in an existing user.
    /// </summary>
    /// <param name="login">The user login details.</param>
    /// <returns>
    /// Returns an OK response with the generated jwt token if the login is successful.
    /// Returns an Unauthorized response with a message "Invalid username or password." if the login fails.
    /// Returns a BadRequest response with the ModelState if the request is invalid.
    /// </returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var token = await authService.LoginUserAsync(login);
        return token != null ? Ok(token) : Unauthorized("Invalid username or password.");
    }
    
    /// <summary>
    /// Refreshes the JWT token using the provided refresh token.
    /// </summary>
    /// <param name="refreshTokenDto">The refresh token to use for token refresh.</param>
    /// <returns>
    /// Returns an OK response with the new JWT token if the refresh is successful.
    /// Returns an Unauthorized response with a message "Invalid refresh token." if the refresh token is invalid.
    /// Returns an Unauthorized response with a message "User associated with refresh token not found." if the user associated with the refresh token is not found.
    /// </returns>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var storedRefreshToken = await unitOfWork.RefreshTokens.GetByTokenAsync(refreshTokenDto.Token);
        if (storedRefreshToken == null || storedRefreshToken.Expires < DateTime.UtcNow)
            return Unauthorized("Invalid refresh token.");
        
        await unitOfWork.RefreshTokens.DeleteAsync(storedRefreshToken);

        var userId = storedRefreshToken.UserId; 
        var user = await unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) 
            return Unauthorized("User associated with refresh token not found.");

        var newTokens = authService.GenerateJwtToken(userId, user.Username, storedRefreshToken.Role);
        
        return Ok(newTokens);
    }
    
    /// <summary>
    /// Sends a password reset email to the specified email address.
    /// </summary>
    /// <param name="email">The email address to send the password reset email to.</param>
    /// <returns>
    /// Returns an Ok response with a message "Password reset email sent successfully." if the email is sent successfully.
    /// Returns a NotFound response with a message "User with this email not found." if the user is not found.
    /// </returns>
    [HttpPost("forgot")]
    public async Task<IActionResult> ForgotPassword([FromQuery] string email)
    {
        var success = await authService.RequestPasswordResetAsync(email);
        return success ? Ok("Password reset email sent successfully.") : NotFound("User with this email not found.");
    }

    /// <summary>
    /// Resets the password for the specified user.
    /// </summary>
    /// <param name="token">The password reset token.</param>
    /// <param name="newPassword">The new password.</param>
    /// <returns>
    /// Returns an Ok response with a message "Password reset successfully." if the password is reset successfully.
    /// Returns a BadRequest response with a message "Invalid or expired password reset token." if the token is invalid or expired.
    /// </returns>
    [HttpPost("reset/{token}/{newPassword}")]
    public async Task<IActionResult> ResetPassword(string token, string newPassword)
    {
        var success = await authService.ResetPasswordAsync(token, newPassword);
        return success ? Ok("Password reset successfully.") : BadRequest("Invalid or expired password reset token.");
    }
}