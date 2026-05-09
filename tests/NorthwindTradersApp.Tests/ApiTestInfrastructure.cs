using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NorthwindTradersApp.Domain.Contracts;

namespace NorthwindTradersApp.Tests;

public sealed class ApiTestFactory : WebApplicationFactory<Program>
{
    public OrdersServiceStub Orders { get; } = new();
    public ProductsServiceStub Products { get; } = new();
    public CustomersServiceStub Customers { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IOrdersService>();
            services.RemoveAll<IProductsService>();
            services.RemoveAll<ICustomersService>();

            services.AddSingleton<IOrdersService>(Orders);
            services.AddSingleton<IProductsService>(Products);
            services.AddSingleton<ICustomersService>(Customers);
        });
    }
}

public sealed class OrdersServiceStub : IOrdersService
{
    public Func<CancellationToken, Task<IEnumerable<OrderDto>>> GetAllOrdersAsyncHandler { get; set; } =
        ct => Task.FromResult<IEnumerable<OrderDto>>(Array.Empty<OrderDto>());

    public Func<int?, string?, CancellationToken, Task<IEnumerable<OrderDto>>> SearchOrderAsyncHandler { get; set; } =
        (orderId, customerName, ct) => Task.FromResult<IEnumerable<OrderDto>>(Array.Empty<OrderDto>());

    public Func<OrderDto, CancellationToken, Task<OrderDto>> CreateOrderAsyncHandler { get; set; } =
        (orderDto, ct) => Task.FromResult(orderDto);

    public Func<int, OrderDto, CancellationToken, Task<OrderDto>> UpdateOrderAsyncHandler { get; set; } =
        (orderId, orderDto, ct) => Task.FromResult(orderDto);

    public Func<int, CancellationToken, Task<bool>> DeleteOrderAsyncHandler { get; set; } =
        (orderId, ct) => Task.FromResult(true);

    public Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default) => GetAllOrdersAsyncHandler(ct);

    public Task<IEnumerable<OrderDto>> SearchOrderAsync(int? OrderId, string? CustomerName, CancellationToken ct = default) =>
        SearchOrderAsyncHandler(OrderId, CustomerName, ct);

    public Task<OrderDto> CreateOrderAsync(OrderDto orderDto, CancellationToken ct = default) =>
        CreateOrderAsyncHandler(orderDto, ct);

    public Task<OrderDto> UpdateOrderAsync(int orderId, OrderDto orderDto, CancellationToken ct = default) =>
        UpdateOrderAsyncHandler(orderId, orderDto, ct);

    public Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct = default) =>
        DeleteOrderAsyncHandler(orderId, ct);
}

public sealed class ProductsServiceStub : IProductsService
{
    public Func<CancellationToken, Task<IEnumerable<ProductDto>>> GetAllProductsAsyncHandler { get; set; } =
        ct => Task.FromResult<IEnumerable<ProductDto>>(Array.Empty<ProductDto>());

    public Func<string?, CancellationToken, Task<IEnumerable<ProductDto>>> SearchProductAsyncHandler { get; set; } =
        (name, ct) => Task.FromResult<IEnumerable<ProductDto>>(Array.Empty<ProductDto>());

    public Func<ProductDto, CancellationToken, Task<ProductDto>> CreateProductAsyncHandler { get; set; } =
        (productDto, ct) => Task.FromResult(productDto);

    public Func<int, ProductDto, CancellationToken, Task<ProductDto>> UpdateProductAsyncHandler { get; set; } =
        (productId, productDto, ct) => Task.FromResult(productDto);

    public Func<int, CancellationToken, Task<bool>> DeleteProductAsyncHandler { get; set; } =
        (productId, ct) => Task.FromResult(true);

    public Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken ct = default) => GetAllProductsAsyncHandler(ct);

    public Task<IEnumerable<ProductDto>> SearchProductAsync(string? name, CancellationToken ct = default) =>
        SearchProductAsyncHandler(name, ct);

    public Task<ProductDto> CreateProductAsync(ProductDto productDto, CancellationToken ct = default) =>
        CreateProductAsyncHandler(productDto, ct);

    public Task<ProductDto> UpdateProductAsync(int productId, ProductDto productDto, CancellationToken ct = default) =>
        UpdateProductAsyncHandler(productId, productDto, ct);

    public Task<bool> DeleteProductAsync(int productId, CancellationToken ct = default) =>
        DeleteProductAsyncHandler(productId, ct);
}

public sealed class CustomersServiceStub : ICustomersService
{
    public Func<CancellationToken, Task<IEnumerable<CustomerDto>>> GetAllCustomersAsyncHandler { get; set; } =
        ct => Task.FromResult<IEnumerable<CustomerDto>>(Array.Empty<CustomerDto>());

    public Func<string?, CancellationToken, Task<IEnumerable<CustomerDto>>> SearchCustomerAsyncHandler { get; set; } =
        (name, ct) => Task.FromResult<IEnumerable<CustomerDto>>(Array.Empty<CustomerDto>());

    public Func<CustomerDto, CancellationToken, Task<CustomerDto>> CreateCustomerAsyncHandler { get; set; } =
        (customerDto, ct) => Task.FromResult(customerDto);

    public Func<string, CustomerDto, CancellationToken, Task<CustomerDto>> UpdateCustomerAsyncHandler { get; set; } =
        (customerId, customerDto, ct) => Task.FromResult(customerDto);

    public Func<string, CancellationToken, Task<bool>> DeleteCustomerAsyncHandler { get; set; } =
        (customerId, ct) => Task.FromResult(true);

    public Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken ct = default) => GetAllCustomersAsyncHandler(ct);

    public Task<IEnumerable<CustomerDto>> SearchCustomerAsync(string? name, CancellationToken ct = default) =>
        SearchCustomerAsyncHandler(name, ct);

    public Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, CancellationToken ct = default) =>
        CreateCustomerAsyncHandler(customerDto, ct);

    public Task<CustomerDto> UpdateCustomerAsync(string customerId, CustomerDto customerDto, CancellationToken ct = default) =>
        UpdateCustomerAsyncHandler(customerId, customerDto, ct);

    public Task<bool> DeleteCustomerAsync(string customerId, CancellationToken ct = default) =>
        DeleteCustomerAsyncHandler(customerId, ct);
}
