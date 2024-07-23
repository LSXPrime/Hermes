using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;

namespace Hermes.API.Tests.Integration.Controllers;

public class ReviewsControllerTests(CustomWebApplicationFactory<Program> factory)
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
    public async Task CreateReview_AuthenticatedUser_ReturnsCreated()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var newReview = new CreateReviewDto
        {
            Rating = 4,
            ReviewText = "Great product! Highly recommend it.",
            ProductId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/Reviews/products/{newReview.ProductId}", newReview);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdReview = await response.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(createdReview);
        Assert.Equal(newReview.Rating, createdReview.Rating);
        Assert.Equal(newReview.ReviewText, createdReview.ReviewText);
        Assert.Equal(newReview.ProductId, createdReview.ProductId);
    }

    [Fact]
    public async Task CreateReview_AuthenticatedSeller_ReturnsCreated()
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

        var newReview = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = "Thank you for your purchase!",
            ProductId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/Reviews/products/{newReview.ProductId}", newReview);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdReview = await response.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(createdReview);
        Assert.Equal(newReview.Rating, createdReview.Rating);
        Assert.Equal(newReview.ReviewText, createdReview.ReviewText);
        Assert.Equal(newReview.ProductId, createdReview.ProductId);
    }

    [Fact]
    public async Task CreateReview_AuthenticatedAdmin_ReturnsCreated()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var newReview = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = "This product is amazing!",
            ProductId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/Reviews/products/{newReview.ProductId}", newReview);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdReview = await response.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(createdReview);
        Assert.Equal(newReview.Rating, createdReview.Rating);
        Assert.Equal(newReview.ReviewText, createdReview.ReviewText);
        Assert.Equal(newReview.ProductId, createdReview.ProductId);
    }

    [Fact]
    public async Task CreateReview_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var newReview = new CreateReviewDto
        {
            Rating = 4,
            ReviewText = "Great product! Highly recommend it.",
            ProductId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/Reviews/products/{newReview.ProductId}", newReview);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetReviewsForProduct_ValidProductId_ReturnsOkWithListOfReviews()
    {
        // Arrange
        int productId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Reviews/products/{productId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var reviews = await response.Content.ReadFromJsonAsync<List<ReviewDto>>();
        Assert.NotNull(reviews);
        Assert.Equal(productId, reviews.FirstOrDefault()!.ProductId);
    }

    [Fact]
    public async Task GetReviewsForProduct_InvalidProductId_ReturnsNotFound()
    {
        // Arrange
        int invalidProductId = 999;

        // Act
        var response = await _client.GetAsync($"/api/Reviews/products/{invalidProductId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetReviewById_ValidId_ReturnsOkWithReview()
    {
        // Arrange
        int reviewId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Reviews/{reviewId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var review = await response.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(review);
        Assert.Equal(reviewId, review.Id);
    }

    [Fact]
    public async Task GetReviewById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        int invalidReviewId = 999;

        // Act
        var response = await _client.GetAsync($"/api/Reviews/{invalidReviewId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateReview_AuthenticatedUser_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        const int reviewId = 1;
        var updatedReview = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = "Updated review text."
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Reviews/{reviewId}", updatedReview);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateReview_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        const int reviewId = 1;
        var updatedReview = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = "Updated review text from Admin."
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Reviews/{reviewId}", updatedReview);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateReview_AuthenticatedSeller_ReturnsForbidden()
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

        const int reviewId = 1;
        var updatedReview = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = "Updated review text from Seller."
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Reviews/{reviewId}", updatedReview);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateReview_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        const int reviewId = 1;
        var updatedReview = new CreateReviewDto
        {
            Rating = 5,
            ReviewText = "Updated review text."
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Reviews/{reviewId}", updatedReview);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteReview_AuthenticatedUser_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Reviews/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteReview_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Reviews/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteReview_AuthenticatedSeller_ReturnsForbidden()
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
        var response = await _client.DeleteAsync("/api/Reviews/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteReview_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Reviews/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}