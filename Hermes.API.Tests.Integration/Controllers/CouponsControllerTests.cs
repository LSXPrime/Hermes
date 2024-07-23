using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;
using Hermes.Domain.Enums;

namespace Hermes.API.Tests.Integration.Controllers;

public class CouponsControllerTests(CustomWebApplicationFactory<Program> factory)
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
    public async Task CreateCoupon_AuthenticatedAdmin_ReturnsCreated()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var newCoupon = new CreateCouponDto
        {
            Code = "TEST20",
            Description = "Test discount coupon",
            CouponType = CouponType.Percentage,
            DiscountAmount = 10,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30),
            MinimumOrderAmount = 50,
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Coupons", newCoupon);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdCoupon = await response.Content.ReadFromJsonAsync<CouponDto>();
        Assert.NotNull(createdCoupon);
        Assert.Equal(newCoupon.Code, createdCoupon!.Code);
        Assert.Equal(newCoupon.Description, createdCoupon.Description);
        Assert.Equal(newCoupon.CouponType, createdCoupon.CouponType);
        Assert.Equal(newCoupon.DiscountAmount, createdCoupon.DiscountAmount);
        Assert.Equal(newCoupon.StartDate, createdCoupon.StartDate);
        Assert.Equal(newCoupon.EndDate, createdCoupon.EndDate);
        Assert.Equal(newCoupon.MinimumOrderAmount, createdCoupon.MinimumOrderAmount);
        Assert.Equal(newCoupon.IsActive, createdCoupon.IsActive);
    }

    [Fact]
    public async Task CreateCoupon_AuthenticatedUser_ReturnsForbidden()
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

        var newCoupon = new CreateCouponDto
        {
            Code = "SUMMER20",
            Description = "Summer discount coupon",
            CouponType = CouponType.Percentage,
            DiscountAmount = 10,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30),
            MinimumOrderAmount = 50,
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Coupons", newCoupon);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCoupon_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var newCoupon = new CreateCouponDto
        {
            Code = "SUMMER20",
            Description = "Summer discount coupon",
            CouponType = CouponType.Percentage,
            DiscountAmount = 10,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30),
            MinimumOrderAmount = 50,
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Coupons", newCoupon);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCoupon_AuthenticatedAdmin_ReturnsOk()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        const int couponId = 1;
        var updatedCoupon = new UpdateCouponDto
        {
            Code = "SUMMER25",
            Description = "Summer discount updated",
            CouponType = CouponType.Percentage,
            DiscountAmount = 15,
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(35),
            MinimumOrderAmount = 60,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Coupons/{couponId}", updatedCoupon);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updatedCouponResult = await response.Content.ReadFromJsonAsync<CouponDto>();
        Assert.NotNull(updatedCouponResult);
        Assert.Equal(updatedCoupon.Code, updatedCouponResult!.Code);
    }

    [Fact]
    public async Task UpdateCoupon_AuthenticatedUser_ReturnsForbidden()
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

        const int couponId = 1;
        var updatedCoupon = new UpdateCouponDto
        {
            Code = "SUMMER25",
            Description = "Summer discount updated",
            CouponType = CouponType.Percentage,
            DiscountAmount = 15,
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(35),
            MinimumOrderAmount = 60,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Coupons/{couponId}", updatedCoupon);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCoupon_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        const int couponId = 1;
        var updatedCoupon = new UpdateCouponDto
        {
            Code = "SUMMER25",
            Description = "Summer discount updated",
            CouponType = CouponType.Percentage,
            DiscountAmount = 15,
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(35),
            MinimumOrderAmount = 60,
            IsActive = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Coupons/{couponId}", updatedCoupon);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCoupon_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Coupons/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCoupon_AuthenticatedUser_ReturnsForbidden()
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
        var response = await _client.DeleteAsync("/api/Coupons/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCoupon_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Coupons/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetActiveCoupons_AuthenticatedAdmin_ReturnsOkWithActiveCoupons()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Coupons/active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var activeCoupons = await response.Content.ReadFromJsonAsync<List<CouponDto>>();
        Assert.NotNull(activeCoupons);
    }

    [Fact]
    public async Task GetActiveCoupons_AuthenticatedUser_ReturnsForbidden()
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
        var response = await _client.GetAsync("/api/Coupons/active");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetActiveCoupons_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Coupons/active");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetExpiredCoupons_AuthenticatedAdmin_ReturnsOkWithExpiredCoupons()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Coupons/expired");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expiredCoupons = await response.Content.ReadFromJsonAsync<List<CouponDto>>();
        Assert.NotNull(expiredCoupons);
    }

    [Fact]
    public async Task GetExpiredCoupons_AuthenticatedUser_ReturnsForbidden()
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
        var response = await _client.GetAsync("/api/Coupons/expired");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetExpiredCoupons_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Coupons/expired");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCouponById_ValidId_ReturnsOkWithCoupon()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);
        int couponId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Coupons/{couponId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var coupon = await response.Content.ReadFromJsonAsync<CouponDto>();
        Assert.NotNull(coupon);
        Assert.Equal(couponId, coupon!.Id);
    }

    [Fact]
    public async Task GetCouponById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);
        int invalidCouponId = 999;

        // Act
        var response = await _client.GetAsync($"/api/Coupons/{invalidCouponId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}