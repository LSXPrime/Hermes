using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;

namespace Hermes.API.Tests.Integration.Controllers;

public class ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
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
    public async Task GetAllProducts_ReturnsOkWithListOfProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/Products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products.Items);
    }

    [Fact]
    public async Task GetProductById_ValidId_ReturnsOkWithProduct()
    {
        // Arrange
        int productId = 1;

        // Act
        var response = await _client.GetAsync($"/api/Products/{productId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal(productId, product!.Id);
    }

    [Fact]
    public async Task GetProductById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        int invalidProductId = 999;

        // Act
        var response = await _client.GetAsync($"/api/Products/{invalidProductId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProductsByCategory_ValidCategoryId_ReturnsOkWithListOfProducts()
    {
        // Arrange
        int categoryId = 4;

        // Act
        var response = await _client.GetAsync($"/api/Products/category/{categoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products.Items);
        Assert.Equal(products.Items.FirstOrDefault()!.CategoryId, categoryId);
    }

    [Fact]
    public async Task GetProductsByCategory_InvalidCategoryId_ReturnsNotFound()
    {
        // Arrange
        int invalidCategoryId = 999;

        // Act
        var response = await _client.GetAsync($"/api/Products/category/{invalidCategoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchProducts_ValidSearchTerm_ReturnsOkWithListOfProducts()
    {
        // Arrange
        string searchTerm = "iPhone";

        // Act
        var response = await _client.GetAsync($"/api/Products/search?searchTerm={searchTerm}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(products);
        Assert.NotEmpty(products.Items);
        Assert.Contains(products.Items, p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchProducts_InvalidSearchTerm_ReturnsOkWithEmptyListOfProducts()
    {
        // Arrange
        string invalidSearchTerm = "nonexistentproduct";

        // Act
        var response = await _client.GetAsync($"/api/Products/search?searchTerm={invalidSearchTerm}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(products);
        Assert.Empty(products.Items);
    }

    [Fact]
    public async Task GetTopSellingProducts_ReturnsOkWithListOfProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/Products/top-selling");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(products);
    }

    [Fact]
    public async Task GetLatestProducts_ReturnsOkWithListOfProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/Products/latest");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var products = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(products);
    }

    [Fact]
    public async Task CreateProduct_AuthenticatedAdmin_ReturnsCreated()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var newProduct = new CreateProductDto
        {
            Name = "New Product",
            Description = "A new product for testing.",
            Price = 19.99m,
            ImageUrl = "https://example.com/new-product.jpg",
            Weight = 1.0,
            WeightUnit = "kg",
            Height = 1.0,
            HeightUnit = "m",
            Width = 1.0,
            WidthUnit = "m",
            Length = 1.0,
            LengthUnit = "m",
            CategoryId = 1,
            SellerId = 1,
            Variants = new List<ProductVariantDto>
            {
                new()
                {
                    SKU = "NEWPRODUCT-VARIANT-1",
                    ImageUrl = "https://example.com/new-product-variant-1.jpg",
                    PriceAdjustment = 19.99m,
                    Quantity = 10,
                    InStock = true,
                    Options = new List<ProductVariantOptionDto>
                    {
                        new() { Name = "Color", Value = "Red" }
                    }
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(createdProduct);
        Assert.Equal(newProduct.Name, createdProduct!.Name);
        Assert.Equal(newProduct.Description, createdProduct.Description);
        Assert.Equal(newProduct.Price, createdProduct.Price);
        Assert.Equal(newProduct.ImageUrl, createdProduct.ImageUrl);
        Assert.Equal(newProduct.CategoryId, createdProduct.CategoryId);
        Assert.Equal(newProduct.SellerId, createdProduct.SellerId);
        Assert.Single(createdProduct.Variants);
        Assert.Equal(newProduct.Variants.First().SKU, createdProduct.Variants[0].SKU);
    }

    [Fact]
    public async Task CreateProduct_AuthenticatedSeller_ReturnsCreated()
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

        var newProduct = new CreateProductDto
        {
            Name = "New Product from Seller",
            Description = "A new product for testing from a seller.",
            Price = 29.99m,
            ImageUrl = "https://example.com/new-product-seller.jpg",
            Weight = 2.0,
            WeightUnit = "kg",
            Height = 2.0,
            HeightUnit = "m",
            Width = 2.0,
            WidthUnit = "m",
            Length = 2.0,
            LengthUnit = "m",
            CategoryId = 1,
            SellerId = 4,
            Variants = new List<ProductVariantDto>
            {
                new()
                {
                    SKU = "NEWPRODUCT-VARIANT-2",
                    ImageUrl = "https://example.com/new-product-variant-1.jpg",
                    PriceAdjustment = 29.99m,
                    Quantity = 15,
                    InStock = true,
                    Options = new List<ProductVariantOptionDto>
                    {
                        new() { Name = "Size", Value = "Large" }
                    }
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(createdProduct);
        Assert.Equal(newProduct.Name, createdProduct!.Name);
        Assert.Equal(newProduct.Description, createdProduct.Description);
        Assert.Equal(newProduct.Price, createdProduct.Price);
        Assert.Equal(newProduct.ImageUrl, createdProduct.ImageUrl);
        Assert.Equal(newProduct.CategoryId, createdProduct.CategoryId);
        Assert.Equal(newProduct.SellerId, createdProduct.SellerId);
        Assert.Single(createdProduct.Variants);
        Assert.Equal(newProduct.Variants.First().SKU, createdProduct.Variants[0].SKU);
    }

    [Fact]
    public async Task CreateProduct_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a regular user (not an admin or seller)
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        var newProduct = new CreateProductDto
        {
            Name = "New Product from User",
            Description = "A new product for testing from a user.",
            Price = 39.99m,
            ImageUrl = "https://example.com/new-product-user.jpg",
            Weight = 2.0,
            WeightUnit = "kg",
            Height = 2.0,
            HeightUnit = "m",
            Width = 2.0,
            WidthUnit = "m",
            Length = 2.0,
            LengthUnit = "m",
            CategoryId = 1,
            SellerId = 1,
            Variants = new List<ProductVariantDto>
            {
                new()
                {
                    SKU = "NEWPRODUCT-VARIANT-3",
                    ImageUrl = "https://example.com/new-product-variant-1.jpg",
                    PriceAdjustment = 39.99m,
                    Quantity = 20,
                    InStock = true,
                    Options = new List<ProductVariantOptionDto>
                    {
                        new() { Name = "Color", Value = "Blue" }
                    }
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var newProduct = new CreateProductDto
        {
            Name = "New Product",
            Description = "A new product for testing.",
            Price = 19.99m,
            ImageUrl = "https://example.com/new-product.jpg",
            Weight = 2.0,
            WeightUnit = "kg",
            Height = 2.0,
            HeightUnit = "m",
            Width = 2.0,
            WidthUnit = "m",
            Length = 2.0,
            LengthUnit = "m",
            CategoryId = 1,
            SellerId = 1,
            Variants = new List<ProductVariantDto>
            {
                new()
                {
                    SKU = "NEWPRODUCT-VARIANT-1",
                    ImageUrl = "https://example.com/new-product-variant-1.jpg",
                    PriceAdjustment = 19.99m,
                    Quantity = 10,
                    InStock = true,
                    Options = new List<ProductVariantOptionDto>
                    {
                        new() { Name = "Color", Value = "Red" }
                    }
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products", newProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        const int productId = 1;
        var updatedProduct = new UpdateProductDto
        {
            Name = "Updated Product Name",
            Description = "Updated product description.",
            Price = 24.99m,
            ImageUrl = "https://example.com/updated-product.jpg"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Products/{productId}", updatedProduct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_AuthenticatedSeller_ReturnsNoContent()
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

        const int productId = 1;
        var updatedProduct = new UpdateProductDto
        {
            Name = "Updated Product Name from Seller",
            Description = "Updated product description from seller.",
            Price = 24.99m,
            ImageUrl = "https://example.com/updated-product-seller.jpg"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Products/{productId}", updatedProduct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a regular user (not an admin or seller)
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        const int productId = 1;
        var updatedProduct = new UpdateProductDto
        {
            Name = "Updated Product Name from User",
            Description = "Updated product description from user.",
            Price = 24.99m,
            ImageUrl = "https://example.com/updated-product-user.jpg"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Products/{productId}", updatedProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        const int productId = 1;
        var updatedProduct = new UpdateProductDto
        {
            Name = "Updated Product Name",
            Description = "Updated product description.",
            Price = 24.99m,
            ImageUrl = "https://example.com/updated-product.jpg"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Products/{productId}", updatedProduct);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Products/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_AuthenticatedSeller_ReturnsNoContent()
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
        var response = await _client.DeleteAsync("/api/Products/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a regular user (not an admin or seller)
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Products/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Products/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProductVariant_AuthenticatedAdmin_ReturnsCreated()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products/variants/1", new CreateProductVariantDto
        {
            SKU = "NEWVARIANT-128GB",
            ImageUrl = "https://example.com/new-product-variant-1.jpg",
            Price = 0,
            Quantity = 50,
            InStock = true,
            Options = new List<ProductVariantOptionDto>
            {
                new() { Name = "Storage", Value = "128GB" }
            }
        });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdVariant = await response.Content.ReadFromJsonAsync<ProductVariantDto>();
        Assert.NotNull(createdVariant);
        Assert.Equal("NEWVARIANT-128GB", createdVariant!.SKU);
    }

    [Fact]
    public async Task CreateProductVariant_AuthenticatedSeller_ReturnsCreated()
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
        var response = await _client.PostAsJsonAsync("/api/Products/variants/1", new CreateProductVariantDto
        {
            SKU = "NEWVARIANT-256GB",
            ImageUrl = "https://example.com/new-product-variant-1.jpg",
            Price = 50,
            Quantity = 25,
            InStock = true,
            Options = new List<ProductVariantOptionDto>
            {
                new() { Name = "Storage", Value = "256GB" }
            }
        });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdVariant = await response.Content.ReadFromJsonAsync<ProductVariantDto>();
        Assert.NotNull(createdVariant);
        Assert.Equal("NEWVARIANT-256GB", createdVariant!.SKU);
    }

    [Fact]
    public async Task CreateProductVariant_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a regular user (not an admin or seller)
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Products/variants/1", new CreateProductVariantDto
        {
            SKU = "NEWVARIANT-512GB",
            ImageUrl = "https://example.com/new-product-variant-1.jpg",
            Price = 100,
            Quantity = 10,
            InStock = true,
            Options = new List<ProductVariantOptionDto>
            {
                new() { Name = "Storage", Value = "512GB" }
            }
        });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateProductVariant_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/Products/variants/1", new CreateProductVariantDto
        {
            SKU = "NEWVARIANT-512GB",
            ImageUrl = "https://example.com/new-product-variant-1.jpg",
            Price = 100,
            Quantity = 10,
            InStock = true,
            Options = new List<ProductVariantOptionDto>
            {
                new() { Name = "Storage", Value = "512GB" }
            }
        });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProductVariant_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PutAsJsonAsync("/api/Products/variants/1", new UpdateProductVariantDto
        {
            SKU = "UPDATEDVARIANT-128GB",
            Price = 0,
            Quantity = 60,
            InStock = true,
            Options = new List<ProductVariantOptionDto>
            {
                new() { Name = "Storage", Value = "128GB" },
                new() { Name = "Color", Value = "Black" }
            }
        });

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProductVariant_AuthenticatedSeller_ReturnsNoContent()
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
        var response = await _client.PutAsJsonAsync("/api/Products/variants/1", new UpdateProductVariantDto
        {
            SKU = "UPDATEDVARIANT-256GB",
            Price = 50,
            Quantity = 30,
            InStock = true,
            Options = new List<ProductVariantOptionDto>
            {
                new() { Name = "Storage", Value = "256GB" },
                new() { Name = "Color", Value = "White" }
            }
        });

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProductVariant_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a regular user (not an admin or seller)
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        // Act
        var response = await _client.PutAsJsonAsync("/api/Products/variants/1", new UpdateProductVariantDto
        {
            SKU = "UPDATEDVARIANT-512GB",
            Price = 100,
            Quantity = 15,
            InStock = true,
            Options = new List<ProductVariantOptionDto>
            {
                new() { Name = "Storage", Value = "512GB" },
                new() { Name = "Color", Value = "Silver" }
            }
        });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProductVariant_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PutAsJsonAsync("/api/Products/variants/1", new UpdateProductVariantDto
        {
            SKU = "UPDATEDVARIANT-512GB",
            Price = 100,
            Quantity = 15,
            InStock = true,
            Options = new List<ProductVariantOptionDto>
            {
                new() { Name = "Storage", Value = "512GB" },
                new() { Name = "Color", Value = "Silver" }
            }
        });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProductVariant_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Products/variants/1"); 

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProductVariant_AuthenticatedSeller_ReturnsNoContent()
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
        var response = await _client.DeleteAsync("/api/Products/variants/1"); 

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProductVariant_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a regular user (not an admin or seller)
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Products/variants/1"); 

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteProductVariant_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Products/variants/1"); 

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UploadProductImage_AuthenticatedAdmin_ReturnsOkWithImageUrl()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var image = await new HttpClient().GetStreamAsync("https://picsum.photos/500");
        var streamContent = new StreamContent(image);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        var form = new MultipartFormDataContent();
        form.Add(streamContent, "imageFile", "image.jpg");

        // Act
        var response = await _client.PostAsync("/api/Products/upload-image", form);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var imageResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(imageResponse);
        Assert.NotEmpty(imageResponse["ImageUrl"]);
    }

    [Fact]
    public async Task UploadProductImage_AuthenticatedSeller_ReturnsOkWithImageUrl()
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

        var image = await new HttpClient().GetStreamAsync("https://picsum.photos/500");
        var streamContent = new StreamContent(image);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        var form = new MultipartFormDataContent();
        form.Add(streamContent, "imageFile", "image.jpg");
    
        // Act
        var response = await _client.PostAsync("/api/Products/upload-image", form);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var imageResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(imageResponse);
        Assert.NotEmpty(imageResponse["ImageUrl"]);
    }

    [Fact]
    public async Task UploadProductImage_AuthenticatedUser_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a regular user (not an admin or seller)
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var userTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(userTokenDto!.AccessToken);

        var image = await new HttpClient().GetStreamAsync("https://picsum.photos/500");
        var streamContent = new StreamContent(image);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        var form = new MultipartFormDataContent();
        form.Add(streamContent, "imageFile", "image.jpg");
    
        // Act
        var response = await _client.PostAsync("/api/Products/upload-image", form);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UploadProductImage_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var image = await new HttpClient().GetStreamAsync("https://picsum.photos/500");
        var streamContent = new StreamContent(image);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        var form = new MultipartFormDataContent();
        form.Add(streamContent, "imageFile", "image.jpg");
    
        // Act
        var response = await _client.PostAsync("/api/Products/upload-image", form);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UploadProductImage_NoImageFile_ReturnsBadRequest()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PostAsync("/api/Products/upload-image", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}