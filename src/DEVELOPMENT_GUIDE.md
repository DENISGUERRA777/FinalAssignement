# 👨‍💻 NorthwindTradersApp - Development Guide

## 📋 Tabla de Contenidos

1. [Project Structure](#project-structure)
2. [Code Organization](#code-organization)
3. [Adding New Features](#adding-new-features)
4. [Service Layer](#service-layer)
5. [Repository Layer](#repository-layer)
6. [API Endpoints](#api-endpoints)
7. [Database Queries](#database-queries)
8. [Error Handling](#error-handling)
9. [Testing](#testing)
10. [Best Practices](#best-practices)

---

## Project Structure

### **4-Layer Clean Architecture**

```
API Layer (Controllers, DTOs)
    ↓
Application Layer (Services)
    ↓
Domain Layer (Entities, Interfaces)
    ↓
Infrastructure Layer (Repositories, DbContext)
    ↓
SQL Server Database
```

### **Dependency Direction**

```
API → Application → Domain ← Infrastructure
      ↓________________________↓
```

- **API** depende de **Application** (Servicios)
- **Application** depende de **Domain** (Interfaces)
- **Infrastructure** implementa interfaces de **Domain**
- Nunca hacia atrás

---

## Code Organization

### **1. Domain Layer**

**Propósito:** Define entidades y contratos

**Estructura:**
```csharp
// NorthwindDbEntities/ - Modelos de BD
public sealed class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
}

// Contracts/ - DTOs para API
public sealed class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string CategoryName { get; set; } // Datos adicionales
}

// Repositories/ - Interfaces
public interface IProductsRepository
{
    Task<List<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> CreateProductAsync(ProductDto productDto);
    // ...
}
```

### **2. Application Layer**

**Propósito:** Lógica de negocio

**Estructura:**
```csharp
// Services/
public sealed class ProductService : IProductsService
{
    private readonly IProductsRepository _repository;

    public ProductService(IProductsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        // Lógica de negocio aquí
        return await _repository.GetAllProductsAsync();
    }
}

// DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductService>();
        return services;
    }
}
```

### **3. Infrastructure Layer**

**Propósito:** Acceso a datos y BD

**Estructura:**
```csharp
// DbContext
public sealed class NorthwindDbContext : DbContext
{
    public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    // ...

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de esquema
        modelBuilder.Entity<Product>().ToTable("Products", "dbo");
    }
}

// Repositories/
public sealed class ProductsRepository : IProductsRepository
{
    private readonly NorthwindDbContext _context;

    public ProductsRepository(NorthwindDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        return products
            .Select(MapToDto)
            .ToList();
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            UnitPrice = product.UnitPrice,
            UnitsInStock = product.UnitsInStock,
            CategoryName = product.Category?.CategoryName ?? string.Empty,
            SupplierId = product.SupplierId
        };
    }
}

// DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NorthwindDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("NorthwindDatabase")));

        services.AddScoped<IProductsRepository, ProductsRepository>();
        return services;
    }
}
```

### **4. API Layer**

**Propósito:** Endpoints HTTP y validación

**Estructura:**
```csharp
// Program.cs - Configuración
var builder = WebApplicationBuilder.CreateBuilder(args);

// Agregar servicios
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Endpoints
app.MapGet("/api/products", GetAllProducts)
   .WithName("GetAllProducts")
   .WithOpenApi();

app.MapPost("/api/products", CreateProduct)
   .WithName("CreateProduct")
   .WithOpenApi();

// Handlers
async Task<IResult> GetAllProducts(
    IProductsService productService,
    int pageNumber = 1,
    int pageSize = 20)
{
    var products = await productService.GetAllProductsAsync();
    return Results.Ok(new { data = products, pageNumber, pageSize });
}

async Task<IResult> CreateProduct(
    ProductDto productDto,
    IProductsService productService)
{
    var created = await productService.CreateProductAsync(productDto);
    return Results.Created($"/api/products/{created.ProductId}", created);
}
```

---

## Adding New Features

### **Ejemplo: Agregar Endpoint de Búsqueda Avanzada de Productos**

**Paso 1: Agregar Método al Repositorio**

```csharp
// Repositories/IProductsRepository.cs
public interface IProductsRepository
{
    Task<List<ProductDto>> SearchProductsAsync(
        string? name = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null);
}

// Repositories/ProductsRepository.cs
public sealed class ProductsRepository : IProductsRepository
{
    public async Task<List<ProductDto>> SearchProductsAsync(
        string? name = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.ProductName.Contains(name));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (minPrice.HasValue)
            query = query.Where(p => p.UnitPrice >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(p => p.UnitPrice <= maxPrice);

        var products = await query.ToListAsync();
        return products.Select(MapToDto).ToList();
    }
}
```

**Paso 2: Agregar Método al Servicio**

```csharp
// Services/IProductsService.cs
public interface IProductsService
{
    Task<List<ProductDto>> SearchProductsAsync(
        string? name = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null);
}

// Services/ProductService.cs
public sealed class ProductService : IProductsService
{
    public async Task<List<ProductDto>> SearchProductsAsync(
        string? name = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        return await _repository.SearchProductsAsync(
            name, categoryId, minPrice, maxPrice);
    }
}
```

**Paso 3: Agregar Endpoint a la API**

```csharp
// Program.cs
app.MapGet("/api/products/advanced-search", AdvancedSearchProducts)
   .WithName("AdvancedSearchProducts")
   .WithOpenApi()
   .Produces<List<ProductDto>>(StatusCodes.Status200OK)
   .Produces(StatusCodes.Status400BadRequest);

async Task<IResult> AdvancedSearchProducts(
    IProductsService productService,
    string? name = null,
    int? categoryId = null,
    decimal? minPrice = null,
    decimal? maxPrice = null)
{
    try
    {
        if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
        {
            return Results.BadRequest(
                new { error = "minPrice cannot be greater than maxPrice" });
        }

        var products = await productService.SearchProductsAsync(
            name, categoryId, minPrice, maxPrice);

        return Results.Ok(new { data = products });
    }
    catch (Exception ex)
    {
        return Results.StatusCode(500);
    }
}
```

---

## Service Layer

### **Principios**

1. **Single Responsibility**: Cada servicio tiene una única responsabilidad
2. **Dependency Injection**: Inyectar repositorios, no crear instancias
3. **Async/Await**: Todas las operaciones I/O son asincrónicas
4. **Error Handling**: Propagar excepciones significativas

### **Template Básico**

```csharp
public interface IProductsService
{
    Task<List<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductRequest request);
    Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request);
    Task<bool> DeleteAsync(int id);
}

public sealed class ProductService : IProductsService
{
    private readonly IProductsRepository _repository;
    private readonly IValidator<CreateProductRequest> _validator;

    public ProductService(
        IProductsRepository repository,
        IValidator<CreateProductRequest> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request)
    {
        // Validar
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Mapear DTO
        var productDto = new ProductDto
        {
            ProductName = request.ProductName,
            UnitPrice = request.UnitPrice,
            // ...
        };

        // Perseguir
        return await _repository.CreateProductAsync(productDto);
    }
}
```

---

## Repository Layer

### **Patrones Comunes**

#### **1. Obtener Todos con Include**

```csharp
public async Task<List<OrderDto>> GetAllOrdersAsync()
{
    var orders = await _context.Orders
        .Include(o => o.OrderDetails)
        .Include(o => o.Customer)
        .Include(o => o.Employee)
        .ToListAsync();

    return orders.Select(MapToDto).ToList();
}
```

#### **2. Búsqueda con LINQ Join**

```csharp
public async Task<List<OrderDto>> SearchOrdersAsync(int? orderId, string? customerName)
{
    var query = _context.Orders.AsQueryable();

    if (orderId.HasValue)
    {
        query = query.Where(o => o.OrderId == orderId);
    }

    if (!string.IsNullOrWhiteSpace(customerName))
    {
        query = query.Join(
            _context.Customers,
            o => o.CustomerId,
            c => c.CustomerId,
            (o, c) => new { Order = o, Customer = c })
            .Where(x => x.Customer.CompanyName.Contains(customerName))
            .Select(x => x.Order);
    }

    var orders = await query
        .Include(o => o.OrderDetails)
        .ToListAsync();

    return orders.Select(MapToDto).ToList();
}
```

#### **3. Bulk Update con ExecuteUpdateAsync**

```csharp
public async Task UpdateProductPriceAsync(decimal percentage)
{
    await _context.Products
        .ExecuteUpdateAsync(s =>
            s.SetProperty(
                p => p.UnitPrice,
                p => p.UnitPrice * (1 + (decimal)(percentage / 100))));
}
```

#### **4. Transacciones**

```csharp
public async Task<OrderDto> CreateOrderWithDetailsAsync(OrderDto orderDto)
{
    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        // Crear orden
        var order = new Order { ... };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Crear detalles
        var details = orderDto.OrderDetails
            .Select(d => new OrderDetail 
            { 
                OrderId = order.OrderId,
                ProductId = d.ProductId,
                // ...
            });

        _context.OrderDetails.AddRange(details);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return MapToDto(order);
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

---

## API Endpoints

### **Convenciones**

1. **Métodos HTTP**:
   - `GET` - Leer datos
   - `POST` - Crear datos
   - `PUT` - Actualizar datos
   - `DELETE` - Eliminar datos

2. **URLs**:
   - `/api/resources` - Colección
   - `/api/resources/{id}` - Elemento específico
   - `/api/resources/{id}/sub-resources` - Subelementos

3. **Status Codes**:
   - `200 OK` - Éxito
   - `201 Created` - Creado
   - `204 No Content` - Sin contenido
   - `400 Bad Request` - Error de validación
   - `404 Not Found` - No encontrado
   - `500 Internal Server Error` - Error del servidor

### **Ejemplo Completo**

```csharp
// GET - Obtener todos
app.MapGet("/api/products", GetAllProducts)
   .WithName("GetAllProducts");

// GET - Buscar
app.MapGet("/api/products/search", SearchProducts)
   .WithName("SearchProducts");

// GET - Por ID
app.MapGet("/api/products/{id}", GetProductById)
   .WithName("GetProductById");

// POST - Crear
app.MapPost("/api/products", CreateProduct)
   .WithName("CreateProduct")
   .Accepts<CreateProductRequest>("application/json");

// PUT - Actualizar
app.MapPut("/api/products/{id}", UpdateProduct)
   .WithName("UpdateProduct");

// DELETE - Eliminar
app.MapDelete("/api/products/{id}", DeleteProduct)
   .WithName("DeleteProduct");
```

---

## Database Queries

### **LINQ Tips**

```csharp
// ✅ BIEN - Deferred execution
var query = _context.Products
    .Where(p => p.UnitPrice > 10)
    .OrderBy(p => p.ProductName);

var results = await query.ToListAsync(); // Aquí se ejecuta

// ❌ MAL - Trae todo a memoria
var products = _context.Products.ToList();
var filtered = products.Where(p => p.UnitPrice > 10).ToList();

// ✅ BIEN - Include para evitar N+1
var orders = await _context.Orders
    .Include(o => o.OrderDetails)
    .Include(o => o.Customer)
    .ToListAsync();

// ❌ MAL - Causa N+1 queries
var orders = await _context.Orders.ToListAsync();
foreach (var order in orders)
{
    var details = order.OrderDetails; // Query adicional
}

// ✅ BIEN - Select proyección
var productNames = await _context.Products
    .Select(p => p.ProductName)
    .ToListAsync();

// ✅ BIEN - Aggregate functions
var count = await _context.Products.CountAsync();
var totalPrice = await _context.Products
    .SumAsync(p => p.UnitPrice);
```

---

## Error Handling

### **Jerarquía de Excepciones**

```csharp
// Base
public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message) { }
}

// Específicas
public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id {id} not found") { }
}

public class DuplicateEntityException : ApplicationException
{
    public DuplicateEntityException(string message)
        : base(message) { }
}
```

### **En Repositorios**

```csharp
public async Task<bool> DeleteProductAsync(int productId)
{
    var product = await _context.Products
        .FirstOrDefaultAsync(p => p.ProductId == productId);

    if (product == null)
    {
        throw new EntityNotFoundException("Product", productId);
    }

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();
    return true;
}
```

### **En Servicios**

```csharp
public async Task<ProductDto> GetProductByIdAsync(int id)
{
    try
    {
        var product = await _repository.GetByIdAsync(id);
        return product ?? throw new EntityNotFoundException("Product", id);
    }
    catch (EntityNotFoundException)
    {
        throw;
    }
    catch (Exception ex)
    {
        throw new ApplicationException($"Error getting product: {ex.Message}");
    }
}
```

### **En API Layer**

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var response = exception switch
        {
            EntityNotFoundException ex => (400, new { error = ex.Message }),
            ValidationException ex => (400, new { error = ex.Message }),
            ApplicationException ex => (500, new { error = ex.Message }),
            _ => (500, new { error = "An unexpected error occurred" })
        };

        context.Response.StatusCode = response.Item1;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response.Item2);
    });
});
```

---

## Testing

### **Unit Testing Pattern**

```csharp
public class ProductServiceTests
{
    private readonly Mock<IProductsRepository> _repositoryMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductsRepository>();
        _service = new ProductService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsProductList()
    {
        // Arrange
        var expectedProducts = new List<ProductDto>
        {
            new() { ProductId = 1, ProductName = "Chai" },
            new() { ProductId = 2, ProductName = "Chang" }
        };
        _repositoryMock
            .Setup(x => x.GetAllProductsAsync())
            .ReturnsAsync(expectedProducts);

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(x => x.GetAllProductsAsync(), Times.Once);
    }
}
```

---

## Best Practices

### **1. Convenciones de Nombres**

```csharp
// ✅ Interfaces con prefijo I
public interface IProductsService { }

// ✅ Métodos asincrónicos con sufijo Async
public async Task<ProductDto> GetProductByIdAsync(int id) { }

// ✅ Métodos que retornan booleanos con prefijo Is/Has/Can
public bool IsProductAvailable() { }
public bool HasUnitsInStock() { }

// ✅ Variables privadas con prefijo _
private readonly IProductsRepository _repository;

// ✅ Clases selladas por defecto
public sealed class ProductService { }
```

### **2. SOLID Principles**

```csharp
// Single Responsibility
// ProductService solo maneja lógica de productos
public sealed class ProductService { }

// Open/Closed
// Usar interfaces para extensión
public interface IProductsService { }

// Liskov Substitution
// Las implementaciones pueden intercambiarse
public sealed class ProductService : IProductsService { }

// Interface Segregation
// Interfaces específicas, no genéricas
public interface IProductsService 
{
    Task<List<ProductDto>> GetAllProductsAsync();
}

// Dependency Inversion
// Depender de abstracciones
public ProductService(IProductsRepository repository)
{
    _repository = repository;
}
```

### **3. Inyección de Dependencias**

```csharp
// ✅ BIEN - DI configuration
builder.Services.AddScoped<IProductsService, ProductService>();

// En endpoints
app.MapGet("/api/products", async (IProductsService service) =>
{
    return await service.GetAllProductsAsync();
});

// ❌ MAL - Crear instancias manualmente
var service = new ProductService();
```

### **4. Null Safety**

```csharp
// ✅ BIEN - Null coalescing
var name = productDto?.ProductName ?? string.Empty;

// ✅ BIEN - Null conditional
var category = product?.Category?.CategoryName;

// ✅ BIEN - Guard clauses
if (product == null)
    throw new EntityNotFoundException("Product", id);

// ❌ MAL - Sin validación
string name = productDto.ProductName; // Null reference exception
```

### **5. Async Best Practices**

```csharp
// ✅ BIEN - Usar ConfigureAwait en libraries
await dbContext.SaveChangesAsync().ConfigureAwait(false);

// ✅ BIEN - No usar Result (puede causar deadlocks)
var product = await _repository.GetByIdAsync(id);

// ❌ MAL
var product = _repository.GetByIdAsync(id).Result; // Deadlock posible

// ✅ BIEN - Pasar CancellationToken
public async Task GetProductAsync(
    int id,
    CancellationToken cancellationToken = default)
{
    return await _context.Products
        .FirstOrDefaultAsync(p => p.ProductId == id, cancellationToken);
}
```

---

## Recursos

- [Microsoft - Clean Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/clean-code/)
- [Microsoft - Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
