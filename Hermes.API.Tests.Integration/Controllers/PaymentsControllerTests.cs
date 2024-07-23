using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Hermes.Application.DTOs;

namespace Hermes.API.Tests.Integration.Controllers;

public class PaymentsControllerTests(CustomWebApplicationFactory<Program> factory)
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
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
    }

    [Fact]
    public async Task CreateCheckoutSession_AuthenticatedUser_ReturnsOkWithSessionId()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response =
            await _client.PostAsync("/api/Payments/create-checkout-session/1", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sessionResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(sessionResponse);
        Assert.NotEmpty(sessionResponse["SessionId"]);
    }

    [Fact]
    public async Task CreateCheckoutSession_AuthenticatedAdmin_ReturnsOkWithSessionId()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response =
            await _client.PostAsync("/api/Payments/create-checkout-session/1", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sessionResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(sessionResponse);
        Assert.NotEmpty(sessionResponse["SessionId"]);
    }

    [Fact]
    public async Task CreateCheckoutSession_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response =
            await _client.PostAsync("/api/Payments/create-checkout-session/1", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCheckoutSession_OrderNotInPendingState_ReturnsBadRequest()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response =
            await _client.PostAsync("/api/Payments/create-checkout-session/2",
                null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePaymentIntent_AuthenticatedUser_ReturnsOkWithClientSecret()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response =
            await _client.PostAsync("/api/Payments/create-payment-intent/1", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paymentIntentResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(paymentIntentResponse);
        Assert.NotEmpty(paymentIntentResponse["ClientSecret"]);
    }

    [Fact]
    public async Task CreatePaymentIntent_AuthenticatedAdmin_ReturnsOkWithClientSecret()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response =
            await _client.PostAsync("/api/Payments/create-payment-intent/1", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var paymentIntentResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(paymentIntentResponse);
        Assert.NotEmpty(paymentIntentResponse["ClientSecret"]);
    }

    [Fact]
    public async Task CreatePaymentIntent_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response =
            await _client.PostAsync("/api/Payments/create-payment-intent/1", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePaymentIntent_OrderNotInPendingState_ReturnsBadRequest()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response =
            await _client.PostAsync("/api/Payments/create-payment-intent/2",
                null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task StripeWebhook_ValidSignature_ReturnsOk()
    {
        // Arrange
        // Create a mock Stripe event JSON
        var eventJson = JsonSerializer.Serialize(new
        {
            id = "evt_1234567890",
            type = "payment_intent.succeeded",
            data = new
            {
                Object = new
                {
                    id = "pi_1234567890",
                    metadata = new
                    {
                        orderId = "1"
                    }
                }
            }
        });
        var signature = ComputeSignature(eventJson, "your_webhook_secret");

        // Act
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Payments/webhook")
        {
            Content = new StringContent(eventJson, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Stripe-Signature", signature);
        var response = await _client.SendAsync(request);

        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task StripeWebhook_InvalidSignature_ReturnsBadRequest()
    {
        // Arrange
        // Create a mock Stripe event JSON
        var eventJson = JsonSerializer.Serialize(new
        {
            id = "evt_1234567890",
            type = "payment_intent.succeeded",
            data = new
            {
                Object = new
                {
                    id = "pi_1234567890",
                    metadata = new
                    {
                        orderId = "1"
                    }
                }
            }
        });
        var invalidSignature = ComputeSignature(eventJson, "wrong_secret");

        // Act
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Payments/webhook")
        {
            Content = new StringContent(eventJson, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("Stripe-Signature", invalidSignature);
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static string ComputeSignature(string payload, string secret)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes($"{DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()}.{payload}");

        using var cryptographer = new HMACSHA256(secretBytes);
        var hash = cryptographer.ComputeHash(payloadBytes);
        return BitConverter.ToString(hash)
            .Replace("-", string.Empty).ToLowerInvariant();
    }
}