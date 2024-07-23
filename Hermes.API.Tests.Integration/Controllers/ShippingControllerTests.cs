using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;

namespace Hermes.API.Tests.Integration.Controllers;

public class ShippingControllerTests(CustomWebApplicationFactory<Program> factory)
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
    public async Task GetShippingRates_ValidRequest_ReturnsOkWithShippingRates()
    {
        // Arrange
        var shippingRateRequests = new List<ShippingRateRequest>
        {
            new()
            {
                OriginPostalCode = "90001", // Los Angeles
                DestinationPostalCode = "62701", // Springfield
                Weight = 1,
                Length = 10,
                Width = 5,
                Height = 5
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Shipping/rates", shippingRateRequests);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var shippingRates = await response.Content.ReadFromJsonAsync<List<ShippingRate>>();
        Assert.NotNull(shippingRates);
        Assert.NotEmpty(shippingRates);
    }

    [Fact]
    public async Task CreateShipment_AuthenticatedAdmin_ReturnsOkWithShipment()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var createShipmentRequests = new List<CreateShipmentRequest>
        {
            new()
            {
                ShippingLabelUrl = "https://example.com/label.png",
                OriginPostalCode = "90001", // Los Angeles
                DestinationPostalCode = "62701", // Springfield
                PackageWeight = 1,
                PackageLength = 10,
                PackageWidth = 5,
                PackageHeight = 5
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Shipping/create-shipment", createShipmentRequests);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var shipment = await response.Content.ReadFromJsonAsync<Shipment>();
        Assert.NotNull(shipment);
        Assert.NotEmpty(shipment!.TrackingNumber);
        Assert.NotEmpty(shipment.ShippingLabelUrls);
    }

    [Fact]
    public async Task CreateShipment_AuthenticatedSeller_ReturnsOkWithShipment()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a seller
        var loginDto = new LoginDto { Username = "seller1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var sellerTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(sellerTokenDto!.AccessToken);

        var createShipmentRequests = new List<CreateShipmentRequest>
        {
            new()
            {
                ShippingLabelUrl = "https://example.com/label.png",
                OriginPostalCode = "90001", // Los Angeles
                DestinationPostalCode = "62701", // Springfield
                PackageWeight = 1,
                PackageLength = 10,
                PackageWidth = 5,
                PackageHeight = 5
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Shipping/create-shipment", createShipmentRequests);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var shipment = await response.Content.ReadFromJsonAsync<Shipment>();
        Assert.NotNull(shipment);
        Assert.NotEmpty(shipment!.TrackingNumber);
        Assert.NotEmpty(shipment.ShippingLabelUrls);
    }

    [Fact]
    public async Task CreateShipment_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a user
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        var createShipmentRequests = new List<CreateShipmentRequest>
        {
            new()
            {
                ShippingLabelUrl = "https://example.com/label.png",
                OriginPostalCode = "90001", // Los Angeles
                DestinationPostalCode = "62701", // Springfield
                PackageWeight = 1,
                PackageLength = 10,
                PackageWidth = 5,
                PackageHeight = 5
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Shipping/create-shipment", createShipmentRequests);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateShipment_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var createShipmentRequests = new List<CreateShipmentRequest>
        {
            new()
            {
                ShippingLabelUrl = "https://example.com/label.png",
                OriginPostalCode = "90001", // Los Angeles
                DestinationPostalCode = "62701", // Springfield
                PackageWeight = 1,
                PackageLength = 10,
                PackageWidth = 5,
                PackageHeight = 5
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Shipping/create-shipment", createShipmentRequests);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TrackShipment_ValidTrackingNumber_ReturnsOkWithTrackingInfo()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);
        string trackingNumber = "123456789";

        // Act
        var response = await _client.GetAsync($"/api/Shipping/track/{trackingNumber}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var trackingInfo = await response.Content.ReadFromJsonAsync<TrackingInformation>();
        Assert.NotNull(trackingInfo);
        Assert.Equal(trackingNumber, trackingInfo!.TrackingNumber);
    }

    [Fact]
    public async Task TrackShipment_InvalidTrackingNumber_ReturnsNotFound()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);
        string invalidTrackingNumber = "invalid_tracking";

        // Act
        var response = await _client.GetAsync($"/api/Shipping/track/{invalidTrackingNumber}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelShipment_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PostAsync("/api/Shipping/cancel/123456789", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CancelShipment_AuthenticatedSeller_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a seller
        var loginDto = new LoginDto { Username = "seller1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var sellerTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(sellerTokenDto!.AccessToken);

        // Act
        var response = await _client.PostAsync("/api/Shipping/cancel/123456789", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CancelShipment_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a user
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        // Act
        var response = await _client.PostAsync("/api/Shipping/cancel/123456789", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CancelShipment_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/Shipping/cancel/123456789", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}