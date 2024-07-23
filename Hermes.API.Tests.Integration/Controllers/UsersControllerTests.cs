using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;

namespace Hermes.API.Tests.Integration.Controllers;

public class UsersControllerTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    // Helper method to get a valid access token for testing
    private async Task<string> GetAccessToken()
    {
        var loginDto = new LoginDto { Username = "admin", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var tokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        return tokenDto!.AccessToken;
    }

    // Helper method to set up Authorization header for authenticated requests
    private void SetAuthorizationHeader(string accessToken)
    {
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    }

    [Fact]
    public async Task GetCurrentUser_AuthenticatedUser_ReturnsOkWithUser()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Users/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal("admin", user!.Username);
    }

    [Fact]
    public async Task GetCurrentUser_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Users/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCurrentUser_AuthenticatedUser_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var updatedUser = new UserDto
        {
            Id = 1,
            Username = "admin",
            Email = "admin@hermes.com",
            FullName = "Updated Admin User",
            PhoneNumber = "1234567890",
            Address = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            Role = "Admin"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/Users/me", updatedUser);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCurrentUser_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var updatedUser = new UserDto
        {
            Id = 1,
            Username = "admin",
            Email = "admin@hermes.com",
            FullName = "Updated Admin User",
            PhoneNumber = "1234567890",
            Address = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            Role = "Admin"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/Users/me", updatedUser);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCurrentUser_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var updatedUser = new UserDto
        {
            Id = 1,
            Username = "admin",
            Email = "admin@hermes.com",
            FullName = "Updated Admin User",
            PhoneNumber = "1234567890",
            Address = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            Role = "Admin"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/Users/me", updatedUser);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUserById_ValidId_ReturnsOkWithUser()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);
        int userId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal(userId, user!.Id);
    }

    [Fact]
    public async Task GetUserById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);
        int invalidUserId = 999;

        // Act
        var response = await _client.GetAsync($"/api/Users/{invalidUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_AuthenticatedAdmin_ReturnsOkWithUsers()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
    }

    [Fact]
    public async Task GetAllUsers_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a regular user (not an admin)
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/Users");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}