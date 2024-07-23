using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;

namespace Hermes.API.Tests.Integration.Controllers;

public class AuthenticationControllerTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Register_ValidUser_ReturnsCreated()
    {
        // Arrange
        var newUser = new RegisterDto
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "@Asd123456",
            ConfirmPassword = "@Asd123456",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Address = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            Role = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/register", newUser);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("User registered successfully.", message);
    }

    [Fact]
    public async Task Register_ExistingUsername_ReturnsBadRequest()
    {
        // Arrange
        var newUser = new RegisterDto
        {
            Username = "admin", // Existing username
            Email = "testuser2@example.com",
            Password = "@@Asd123456",
            ConfirmPassword = "@@Asd123456",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Address = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            Role = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/register", newUser);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Username or email is already taken.", message);
    }

    [Fact]
    public async Task Register_ExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var newUser = new RegisterDto
        {
            Username = "admin",
            Email = "admin@hermes.com", // Existing email
            Password = "@Asd123456",
            ConfirmPassword = "@Asd123456",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Address = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            Role = "Admin"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/register", newUser);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Username or email is already taken.", message);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var login = new LoginDto
        {
            Username = "admin",
            Password = "@Asd123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/login", login);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokenDto = await response.Content.ReadFromJsonAsync<JwtTokenDto>();
        Assert.NotNull(tokenDto);
        Assert.NotEmpty(tokenDto.AccessToken);
        Assert.NotEmpty(tokenDto.RefreshToken);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var login = new LoginDto
        {
            Username = "admin",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/login", login);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Invalid username or password.", message);
    }
    
    [Fact]
    public async Task RefreshToken_ValidRefreshToken_ReturnsOkWithNewToken()
    {
        // Arrange
        var login = new LoginDto { Username = "admin", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", login);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        Assert.NotNull(loginTokenDto);

        // 2. Create a RefreshTokenDto with the obtained refresh token
        var refreshTokenDto = new RefreshTokenDto { Token = loginTokenDto.RefreshToken };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/refresh", refreshTokenDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var newTokenDto = await response.Content.ReadFromJsonAsync<JwtTokenDto>();
        Assert.NotNull(newTokenDto);
        Assert.NotEmpty(newTokenDto.AccessToken);
        Assert.NotEmpty(newTokenDto.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_InvalidRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshTokenDto = new RefreshTokenDto { Token = "invalid_refresh_token" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Authentication/refresh", refreshTokenDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Invalid refresh token.", message);
    }

    [Fact]
    public async Task ForgotPassword_ValidEmail_ReturnsOk()
    {
        // Arrange
        var email = "admin@hermes.com";

        // Act
        var response = await _client.PostAsync($"/api/Authentication/forgot?email={email}", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Password reset email sent successfully.", message);
    }

    [Fact]
    public async Task ForgotPassword_InvalidEmail_ReturnsNotFound()
    {
        // Arrange
        var email = "invalid@email.com";

        // Act
        var response = await _client.PostAsync($"/api/Authentication/forgot?email={email}", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("User with this email not found.", message);
    }

    [Fact]
    public async Task ResetPassword_ValidTokenAndNewPassword_ReturnsOk()
    {
        // Arrange
        var resetToken = "1234567890";
        
        var newPassword = "NewPassword123";

        // Act
        var response = await _client.PostAsync($"/api/Authentication/reset/{resetToken}/{newPassword}", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Password reset successfully.", message);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var token = "invalid_token";
        var newPassword = "NewPassword123";

        // Act
        var response = await _client.PostAsync($"/api/Authentication/reset/{token}/{newPassword}", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Invalid or expired password reset token.", message);
    }
}