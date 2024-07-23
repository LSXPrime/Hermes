using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;

namespace Hermes.API.Tests.Integration.Controllers;

public class CategoriesControllerTests(CustomWebApplicationFactory<Program> factory)
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
    public async Task GetAllCategories_ReturnsOkWithListOfCategories()
    {
        // Act
        var response = await _client.GetAsync("/api/Categories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
    }

    [Fact]
    public async Task GetCategoryById_ValidId_ReturnsOkWithCategory()
    {
        // Arrange
        int categoryId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Categories/{categoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(category);
        Assert.Equal(categoryId, category!.Id);
    }

    [Fact]
    public async Task GetCategoryById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        int invalidCategoryId = 999;

        // Act
        var response = await _client.GetAsync($"/api/Categories/{invalidCategoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSubcategories_ValidCategoryId_ReturnsOkWithListOfSubcategories()
    {
        // Arrange
        int categoryId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Categories/{categoryId}/subcategories");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var subcategories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(subcategories);
        Assert.NotEmpty(subcategories);
    }

    [Fact]
    public async Task GetSubcategories_InvalidCategoryId_ReturnsNotFound()
    {
        // Arrange
        int invalidCategoryId = 999;

        // Act
        var response = await _client.GetAsync($"/api/Categories/{invalidCategoryId}/subcategories");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_AuthenticatedAdmin_ReturnsCreated()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var newCategory = new CreateCategoryDto
        {
            Name = "New Category",
            Description = "A new category for testing.",
            ParentCategoryId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Categories", newCategory);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdCategory = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(createdCategory);
        Assert.Equal(newCategory.Name, createdCategory!.Name);
        Assert.Equal(newCategory.Description, createdCategory.Description);
        Assert.Equal(newCategory.ParentCategoryId, createdCategory.ParentCategoryId);
    }

    [Fact]
    public async Task CreateCategory_AuthenticatedUser_ReturnsForbidden()
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

        var newCategory = new CreateCategoryDto
        {
            Name = "New Category",
            Description = "A new category for testing.",
            ParentCategoryId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Categories", newCategory);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var newCategory = new CreateCategoryDto
        {
            Name = "New Category",
            Description = "A new category for testing.",
            ParentCategoryId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Categories", newCategory);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var updatedCategory = new UpdateCategoryDto
        {
            Id = 1,
            Name = "Updated Electronics",
            Description = "Updated description for Electronics category.",
            ParentCategoryId = null
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Categories/{updatedCategory.Id}", updatedCategory);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_AuthenticatedUser_ReturnsForbidden()
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

        var updatedCategory = new UpdateCategoryDto
        {
            Id = 1,
            Name = "Updated Electronics",
            Description = "Updated description for Electronics category.",
            ParentCategoryId = null
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Categories/{updatedCategory.Id}", updatedCategory);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCategory_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var updatedCategory = new UpdateCategoryDto
        {
            Id = 1,
            Name = "Updated Electronics",
            Description = "Updated description for Electronics category.",
            ParentCategoryId = null
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Categories/{updatedCategory.Id}", updatedCategory);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Categories/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_AuthenticatedUser_ReturnsForbidden()
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
        var response = await _client.DeleteAsync("/api/Categories/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Categories/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}