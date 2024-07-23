using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Hermes.Application.DTOs;
using Hermes.Domain.Enums;

namespace Hermes.API.Tests.Integration.Controllers;

public class OrdersControllerTests(CustomWebApplicationFactory<Program> factory)
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
    public async Task GetOrderPreview_AuthenticatedUser_ReturnsOkWithOrderPreview()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var orderDto = new CreateOrderDto
        {
            UserId = 1,
            ShippingAddress = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            BillingAddress = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductVariantId = 1, Quantity = 2 }
            },
            PaymentMethod = "card",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Orders/preview", orderDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orderPreview = await response.Content.ReadFromJsonAsync<OrderPreviewDto>();
        Assert.NotNull(orderPreview);
        Assert.Equal(orderDto.UserId, orderPreview!.UserId);
        Assert.Equal(orderDto.ShippingAddress.Street, orderPreview.ShippingAddress.Street);
        Assert.Equal(orderDto.BillingAddress.Street, orderPreview.BillingAddress.Street);
        Assert.Single(orderPreview.OrderItems);
    }

    [Fact]
    public async Task GetOrderPreview_AuthenticatedAdmin_ReturnsOkWithOrderPreview()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var orderDto = new CreateOrderDto
        {
            UserId = 1,
            ShippingAddress = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            BillingAddress = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductVariantId = 1, Quantity = 2 }
            },
            PaymentMethod = "card",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Orders/preview", orderDto);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orderPreview = await response.Content.ReadFromJsonAsync<OrderPreviewDto>();
        Assert.NotNull(orderPreview);
        Assert.Equal(orderDto.UserId, orderPreview!.UserId);
        Assert.Equal(orderDto.ShippingAddress.Street, orderPreview.ShippingAddress.Street);
        Assert.Equal(orderDto.BillingAddress.Street, orderPreview.BillingAddress.Street);
        Assert.Single(orderPreview.OrderItems);
    }

    [Fact]
    public async Task GetOrderPreview_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var orderDto = new CreateOrderDto
        {
            UserId = 1,
            ShippingAddress = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            BillingAddress = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductVariantId = 1, Quantity = 2 }
            },
            PaymentMethod = "card",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Orders/preview", orderDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_AuthenticatedUser_ReturnsCreated()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var orderDto = new CreateOrderDto
        {
            UserId = 1,
            ShippingAddress = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            BillingAddress = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductVariantId = 1, Quantity = 2 }
            },
            PaymentMethod = "card",
            ShippingMethod = "DHL",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Orders", orderDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(createdOrder);
        Assert.Equal(orderDto.UserId, createdOrder!.UserId);
        Assert.Equal(orderDto.ShippingAddress.Street, createdOrder.ShippingAddress.Street);
        Assert.Equal(orderDto.BillingAddress.Street, createdOrder.BillingAddress.Street);
        Assert.Single(createdOrder.OrderItems);
        Assert.Equal(OrderStatus.Pending, createdOrder.OrderStatus);
    }

    [Fact]
    public async Task CreateOrder_AuthenticatedAdmin_ReturnsCreated()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var orderDto = new CreateOrderDto
        {
            UserId = 1,
            ShippingAddress = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            BillingAddress = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductVariantId = 1, Quantity = 2 }
            },
            PaymentMethod = "card",
            ShippingMethod = "DHL"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Orders", orderDto);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(createdOrder);
        Assert.Equal(orderDto.UserId, createdOrder.UserId);
        Assert.Equal(orderDto.ShippingAddress.Street, createdOrder.ShippingAddress.Street);
        Assert.Equal(orderDto.BillingAddress.Street, createdOrder.BillingAddress.Street);
        Assert.Single(createdOrder.OrderItems);
        Assert.Equal(OrderStatus.Pending, createdOrder.OrderStatus);
    }

    [Fact]
    public async Task CreateOrder_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var orderDto = new CreateOrderDto
        {
            UserId = 1,
            ShippingAddress = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            BillingAddress = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            OrderItems = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductVariantId = 1, Quantity = 2 }
            },
            PaymentMethod = "card",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Orders", orderDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_EmptyOrderItems_ReturnsBadRequest()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        var orderDto = new CreateOrderDto
        {
            UserId = 1,
            ShippingAddress = new AddressDto
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "US"
            },
            BillingAddress = new AddressDto
            {
                Street = "456 Elm St",
                City = "Springfield",
                State = "IL",
                PostalCode = "62701",
                Country = "US"
            },
            OrderItems = new List<OrderItemDto>(),
            PaymentMethod = "card",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Orders", orderDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("Order must contain at least one item.", errorMessage);
    }

    [Fact]
    public async Task GetAllOrders_AuthenticatedAdmin_ReturnsOkWithOrders()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Orders");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<PagedResult<OrderDto>>();
        Assert.NotNull(orders);
    }

    [Fact]
    public async Task GetAllOrders_AuthenticatedUser_ReturnsForbidden()
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
        var response = await _client.GetAsync("/api/Orders");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAllOrders_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Orders");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetOrdersByUser_AuthenticatedUser_ReturnsOkWithOrders()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Orders/user");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        Assert.NotNull(orders);
    }

    [Fact]
    public async Task GetOrdersByUser_AuthenticatedAdmin_ReturnsOkWithOrders()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Orders/user");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        Assert.NotNull(orders);
    }

    [Fact]
    public async Task GetOrdersByUser_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/Orders/user");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetOrderById_ValidId_ReturnsOkWithOrder()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Orders/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(order);
        Assert.Equal(1, order.Id);
    }

    [Fact]
    public async Task GetOrderById_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.GetAsync("/api/Orders/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelOrder_AuthenticatedUser_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PostAsync("/api/Orders/1/cancel", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CancelOrder_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PostAsync("/api/Orders/1/cancel", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CancelOrder_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/Orders/1/cancel", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CancelOrder_OrderAlreadyCancelled_ReturnsBadRequest()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var cancelResponse = await _client.PostAsync("/api/Orders/1/cancel", null);

        // Assert that the first cancellation is successful
        Assert.Equal(HttpStatusCode.NoContent, cancelResponse.StatusCode);

        // Try to cancel the same order again
        var response = await _client.PostAsync("/api/Orders/1/cancel", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderStatus_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PutAsync("/api/Orders/1/status?newStatus=Paid", null);
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderStatus_AuthenticatedSeller_ReturnsForbidden()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Log in as a seller
        var loginDto = new LoginDto { Username = "user1", Password = "@Asd123456" };
        var loginResponse = await _client.PostAsJsonAsync("/api/Authentication/login", loginDto);
        loginResponse.EnsureSuccessStatusCode();
        var sellerTokenDto = await loginResponse.Content.ReadFromJsonAsync<JwtTokenDto>();
        SetAuthorizationHeader(sellerTokenDto!.AccessToken);

        // Act
        var response = await _client.PutAsync("/api/Orders/1/status?newStatus=Paid", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderStatus_AuthenticatedUser_ReturnsForbidden()
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
        var response = await _client.PutAsync("/api/Orders/1/status?newStatus=Processing", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderStatus_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.PutAsync("/api/Orders/1/status?newStatus=Processing", null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateOrderStatus_InvalidStatus_ReturnsBadRequest()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.PutAsync("/api/Orders/1/status?newStatus=InvalidStatus", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteOrder_AuthenticatedAdmin_ReturnsNoContent()
    {
        // Arrange
        var accessToken = await GetAccessToken();
        SetAuthorizationHeader(accessToken);

        // Act
        var response = await _client.DeleteAsync("/api/Orders/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteOrder_AuthenticatedUser_ReturnsForbidden()
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
        var response = await _client.DeleteAsync("/api/Orders/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteOrder_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Orders/1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}