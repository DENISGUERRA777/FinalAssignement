using NorthwindTradersApp.Application;
using NorthwindTradersApp.Infrastructure;
using NorthwindTradersApp.Domain.Contracts;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "NorthwindTradersApp API", Version = "v1" });
});

// Register application services
builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NorthwindTradersApp API v1");
    });
}

//Orders endpoints
///////////////////////////////////////////////////////////////

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
.WithName("Health")
.WithTags("Health");

// Get all orders with pagination
app.MapGet("/api/orders", async (
    [FromServices]IOrdersService ordersService,
    CancellationToken ct,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 20;
    var orders = await ordersService.GetAllOrdersAsync(ct);
    var totalCount = orders.Count();
    var items = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    var response = new
    {
        page,
        pageSize,
        totalCount,
        items
    };
    return Results.Ok(response);
})
.WithName("GetAllOrders")
.WithTags("Orders");

// Search orders by orderId and/or customerName
app.MapGet("/api/orders/search", async (
    [FromServices]IOrdersService ordersService,
    CancellationToken ct,
    [FromQuery] int? orderId,
    [FromQuery] string? customerName) =>
{
    var orders = await ordersService.SearchOrderAsync(orderId, customerName, ct);
    return Results.Ok(orders);
})
.WithName("SearchOrders")
.WithTags("Orders");

// Create a new order
app.MapPost("/api/orders", async (
    [FromServices]IOrdersService ordersService,
    CancellationToken ct,
    [FromBody] OrderDto orderDto) =>
{
    var createdOrder = await ordersService.CreateOrderAsync(orderDto, ct);
    return Results.Created($"/api/orders/{createdOrder.OrderId}", createdOrder);
})
.WithName("CreateOrder")
.WithTags("Orders");

// Update an existing order
app.MapPut("/api/orders/{orderId}", async (
    [FromServices]IOrdersService ordersService,
    CancellationToken ct,
    [FromRoute] int orderId,
    [FromBody] OrderDto orderDto) =>
{
    try
    {
        var updatedOrder = await ordersService.UpdateOrderAsync(orderId, orderDto, ct);
        return Results.Ok(updatedOrder);
    } 
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
    
})
.WithName("UpdateOrder")
.WithTags("Orders");

// Delete an order
app.MapDelete("/api/orders/{orderId}", async (
    [FromServices]IOrdersService ordersService,
    CancellationToken ct,
    [FromRoute] int orderId) =>
{
    try
    {
        await ordersService.DeleteOrderAsync(orderId, ct);
        return Results.NoContent();
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
})
.WithName("DeleteOrder")
.WithTags("Orders");

// Products endpoints
///////////////////////////////////////////////////////////////

// Get all products with pagination
app.MapGet("/api/products", async (
    [FromServices]IProductsService productsService,
    CancellationToken ct,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 20;
    var products = await productsService.GetAllProductsAsync(ct);
    var totalCount = products.Count();
    var items = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    var response = new
    {
        page,
        pageSize,
        totalCount,
        items
    };
    return Results.Ok(response);
})
.WithName("GetAllProducts")
.WithTags("Products");

// Search products by name
app.MapGet("/api/products/search", async (
    [FromServices]IProductsService productsService,
    CancellationToken ct,
    [FromQuery] string? name) =>
{
    var products = await productsService.SearchProductAsync(name, ct);
    return Results.Ok(products);
})
.WithName("SearchProducts")
.WithTags("Products");

// Create a new product
app.MapPost("/api/products", async (
    [FromServices]IProductsService productsService,
    CancellationToken ct,
    [FromBody] ProductDto productDto) =>
{
    var createdProduct = await productsService.CreateProductAsync(productDto, ct);
    return Results.Created($"/api/products/{createdProduct.ProductId}", createdProduct);
})
.WithName("CreateProduct")
.WithTags("Products");

// Update an existing product
app.MapPut("/api/products/{productId}", async (
    [FromServices]IProductsService productsService,
    CancellationToken ct,
    [FromRoute] int productId,
    [FromBody] ProductDto productDto) =>
{
    try
    {
        var updatedProduct = await productsService.UpdateProductAsync(productId, productDto, ct);
        return Results.Ok(updatedProduct);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
})
.WithName("UpdateProduct")
.WithTags("Products");

// Delete a product
app.MapDelete("/api/products/{productId}", async (
    [FromServices]IProductsService productsService,
    CancellationToken ct,
    [FromRoute] int productId) =>
{
    try
    {
        await productsService.DeleteProductAsync(productId, ct);
        return Results.NoContent();
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
})
.WithName("DeleteProduct")
.WithTags("Products");

// Customer endpoints
///////////////////////////////////////////////////////////////

/// get all customers with pagination
app.MapGet("/api/customers", async (
    [FromServices]ICustomersService customersService,
    CancellationToken ct,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1) pageSize = 20;
    var customers = await customersService.GetAllCustomersAsync(ct);
    var totalCount = customers.Count();
    var items = customers.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    var response = new
    {
        page,
        pageSize,
        totalCount,
        items
    };
    return Results.Ok(response);
})
.WithName("GetAllCustomers")
.WithTags("Customers");

// Search customers by name
app.MapGet("/api/customers/search", async (
    [FromServices]ICustomersService customersService,
    CancellationToken ct,
    [FromQuery] string? name) =>
{
    var customers = await customersService.SearchCustomerAsync(name, ct);
    return Results.Ok(customers);
})
.WithName("SearchCustomers")
.WithTags("Customers");

// Create a new customer
app.MapPost("/api/customers", async (
    [FromServices]ICustomersService customersService,
    CancellationToken ct,
    [FromBody] CustomerDto customerDto) =>
{
    var createdCustomer = await customersService.CreateCustomerAsync(customerDto, ct);
    return Results.Created($"/api/customers/{createdCustomer.CustomerId}", createdCustomer);
})
.WithName("CreateCustomer")
.WithTags("Customers");

// Update an existing customer
app.MapPut("/api/customers/{customerId}", async (
    [FromServices]ICustomersService customersService,
    CancellationToken ct,
    [FromRoute] string customerId,
    [FromBody] CustomerDto customerDto) =>
{
    try
    {
        var updatedCustomer = await customersService.UpdateCustomerAsync(customerId, customerDto, ct);
        return Results.Ok(updatedCustomer);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
})
.WithName("UpdateCustomer")
.WithTags("Customers");

// Delete a customer
app.MapDelete("/api/customers/{customerId}", async (
    [FromServices]ICustomersService customersService,
    CancellationToken ct,
    [FromRoute] string customerId) =>
{
    try
    {
        await customersService.DeleteCustomerAsync(customerId, ct);
        return Results.NoContent();
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
})
.WithName("DeleteCustomer")
.WithTags("Customers");

app.Run();
