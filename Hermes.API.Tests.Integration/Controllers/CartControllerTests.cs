using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;

namespace Hermes.API.Tests.Integration.Controllers;

public class CartControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CartControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

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
    public async Task GetCart_AuthenticatedUser_ReturnsCart()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Cart");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
    }

    [Fact]
    public async Task GetCart_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Cart");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddItemToCart_AuthenticatedUser_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PostAsync("/api/Cart/items/1/2", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task AddItemToCart_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/Cart/items/1/2", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCartItemQuantity_AuthenticatedUser_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PutAsync("/api/Cart/items/1/3", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCartItemQuantity_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PutAsync("/api/Cart/items/1/3", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RemoveItemFromCart_AuthenticatedUser_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Cart/items/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RemoveItemFromCart_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Cart/items/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ClearCart_AuthenticatedUser_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Cart/clear");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ClearCart_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Cart/clear");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ApplyCoupon_AuthenticatedUser_ReturnsOkWithUpdatedCart()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PostAsync("/api/Cart/coupons/WELCOME10", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Equal("WELCOME10", cart.AppliedCouponCode);
    }

    [Fact]
    public async Task ApplyCoupon_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/Cart/coupons/WELCOME10", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RemoveCoupon_AuthenticatedUser_ReturnsOkWithUpdatedCart()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Cart/coupons");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Null(cart.AppliedCouponCode);
    }

    [Fact]
    public async Task RemoveCoupon_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Cart/coupons");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}