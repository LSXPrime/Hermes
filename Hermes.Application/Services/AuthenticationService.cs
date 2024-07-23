using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Hermes.Domain.Settings;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hermes.Application.Services;

public class AuthService(IUnitOfWork unitOfWork, IEmailService emailService, IMapper mapper, IOptions<JwtSettings> options)
    : IAuthService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token if successful.
    /// </summary>
    /// <param name="login">The login credentials.</param>
    /// <returns>The JWT token DTO containing the access token, and expiration time, or null if authentication failed.</returns>
    public async Task<JwtTokenDto?> LoginUserAsync(LoginDto login)
    {
        var user = await unitOfWork.Users.GetByUsernameAsync(login.Username);
        if (user == null || !VerifyPassword(login.Password, user.PasswordHash))
        {
            return null;
        }

        var token = GenerateJwtToken(user.Id, user.Username, user.Role);

        return token;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="newUser">The new user registration details.</param>
    /// <returns>True if registration was successful, false otherwise.</returns>
    public async Task<bool> RegisterAsync(RegisterDto newUser)
    {
        if (await unitOfWork.Users.UserExistsAsync(newUser.Username))
        {
            return false;
        }

        if (await unitOfWork.Users.GetByEmailAsync(newUser.Email) != null)
        {
            return false;
        }

        var user = new User
        {
            Username = newUser.Username,
            Email = newUser.Email,
            PasswordHash = HashPassword(newUser.Password),
            Address = mapper.Map<Address>(newUser.Address),
            PhoneNumber = newUser.PhoneNumber,
            Role = newUser.Role,
            FirstName = newUser.FirstName,
            LastName = newUser.LastName
        };

        await unitOfWork.Users.AddAsync(user);
        return true;
    }

    /// <summary>
    /// Resets the password for the specified user.
    /// </summary>
    /// <param name="token">The password reset token.</param>
    /// <param name="newPassword">The new password.</param>
    /// <returns>True if the password was reset successfully, false otherwise.</returns>
    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var user = (await unitOfWork.Users.ExecuteQueryAsync(
            unitOfWork.Users.FindAsync(u => u.PasswordResetToken == token))).FirstOrDefault();
        if (user == null || user.PasswordResetTokenExpiration < DateTime.UtcNow)
        {
            return false;
        }

        user.PasswordHash = HashPassword(newPassword);

        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiration = null;

        await unitOfWork.Users.UpdateAsync(user);
        return true;
    }

    /// <summary>
    /// Sends a password reset email to the specified email address.
    /// </summary>
    /// <param name="email">The email address to send the password reset email to.</param>
    /// <returns>True if the email was sent successfully, false otherwise.</returns>
    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        var resetToken = GeneratePasswordResetToken(user);

        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(1);
        await unitOfWork.Users.UpdateAsync(user);

        await emailService.SendPasswordResetEmailAsync(user.Email, resetToken);
        return true;
    }

    /// <summary>
    /// Generates a JWT and refresh token for a given user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="userName">The username of the user.</param>
    /// <param name="role">The role of the user.</param>
    /// <returns>The JWT token DTO containing the access token, refresh token, and expiration times.</returns>
    public JwtTokenDto GenerateJwtToken(int userId, string userName, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(options.Value.Secret);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, userName),
                new(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddDays(options.Value.AccessTokenExpirationInDays),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var accessToken = tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = GenerateRefreshToken();
        var refreshExpirationDate = DateTime.UtcNow.AddDays(options.Value.RefreshTokenExpirationInDays);

        unitOfWork.RefreshTokens.AddAsync(new RefreshToken
            { UserId = userId, Token = refreshToken, Expires = refreshExpirationDate, Role = role });
        
        return new JwtTokenDto
        {
            AccessToken = tokenHandler.WriteToken(accessToken),
            AccessTokenExpiration = tokenDescriptor.Expires.Value,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = refreshExpirationDate
        };
    }

    /// <summary>
    /// Generates a random refresh token.
    /// </summary>
    /// <returns>The generated refresh token.</returns>
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Validates a JWT token and returns the associated ClaimsPrincipal if valid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The ClaimsPrincipal associated with the token, or null if the token is invalid.</returns>
    public Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(options.Value.Secret);
            var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true
            }, out _);

            return Task.FromResult(claimsPrincipal)!;
        }
        catch
        {
            return Task.FromResult<ClaimsPrincipal>(null!)!;
        }
    }

    /// <summary>
    /// Hashes a password using PBKDF2 with HMACSHA256.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password as a base64-encoded string.</returns>
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(options.Value.Salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));
    }

    /// <summary>
    /// Verifies a password against a hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the passwords match, false otherwise.</returns>
    private bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }

    /// <summary>
    /// Generates a password reset token for a user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>The generated password reset token.</returns>
    private string GeneratePasswordResetToken(User user)
    {
        var tokenBase = $"{user.Id}-{user.Username}-{user.Guid}";
        return HashPassword(tokenBase).Split('=')[0].Replace('+', '-').Replace('/', '_');
    }
}