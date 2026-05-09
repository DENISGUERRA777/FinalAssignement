using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using NorthwindTradersApp.Domain.Contracts;
using Xunit;

namespace NorthwindTradersApp.Tests;

public sealed class ApiEndpointTests : IClassFixture<ApiTestFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ApiTestFactory _factory;

    public ApiEndpointTests(ApiTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthyStatus()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("healthy", document.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task GetOrdersEndpoint_AppliesPaginationDefaults()
    {
        _factory.Orders.GetAllOrdersAsyncHandler = _ => Task.FromResult<IEnumerable<OrderDto>>(
            new[] { TestData.CreateOrder(1), TestData.CreateOrder(2), TestData.CreateOrder(3) });

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/orders?page=0&pageSize=0");

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(1, document.RootElement.GetProperty("page").GetInt32());
        Assert.Equal(20, document.RootElement.GetProperty("pageSize").GetInt32());
        Assert.Equal(3, document.RootElement.GetProperty("totalCount").GetInt32());
        Assert.Equal(3, document.RootElement.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public async Task SearchProductsEndpoint_ReturnsMatchedProducts()
    {
        _factory.Products.SearchProductAsyncHandler = (name, ct) => Task.FromResult<IEnumerable<ProductDto>>(
            new[] { TestData.CreateProduct(1), TestData.CreateProduct(2) });

        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products/search?name=chai");

        response.EnsureSuccessStatusCode();

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOptions);
        Assert.NotNull(products);
        Assert.Equal(2, products!.Count);
        Assert.All(products, product => Assert.Equal("Chai", product.ProductName));
    }

    [Fact]
    public async Task CreateCustomerEndpoint_ReturnsCreatedResponse()
    {
        _factory.Customers.CreateCustomerAsyncHandler = (customerDto, ct) =>
        {
            customerDto.CustomerId = "ALFKI";
            return Task.FromResult(customerDto);
        };

        using var client = _factory.CreateClient();
        var request = TestData.CreateCustomer();
        request.CustomerId = string.Empty;

        var response = await client.PostAsJsonAsync("/api/customers", request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/customers/ALFKI", response.Headers.Location?.OriginalString);

        var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerDto>(JsonOptions);
        Assert.NotNull(createdCustomer);
        Assert.Equal("ALFKI", createdCustomer!.CustomerId);
    }

    [Fact]
    public async Task UpdateOrderEndpoint_ReturnsNotFoundWhenOrderIsMissing()
    {
        _factory.Orders.UpdateOrderAsyncHandler = (orderId, orderDto, ct) =>
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");

        using var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/orders/99", TestData.CreateOrder(99), JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Order with ID 99 not found.", document.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task DeleteProductEndpoint_ReturnsNoContent()
    {
        _factory.Products.DeleteProductAsyncHandler = (productId, ct) => Task.FromResult(true);

        using var client = _factory.CreateClient();

        var response = await client.DeleteAsync("/api/products/10");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
