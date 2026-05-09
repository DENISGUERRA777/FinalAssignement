# 🔧 NorthwindTradersApp - Troubleshooting Guide

## 📋 Tabla de Contenidos

1. [Database Connection Issues](#database-connection-issues)
2. [Build & Compilation Errors](#build--compilation-errors)
3. [Runtime Errors](#runtime-errors)
4. [API Issues](#api-issues)
5. [Performance Issues](#performance-issues)
6. [Common Exceptions](#common-exceptions)
7. [Debugging Tips](#debugging-tips)

---

## Database Connection Issues

### **Error: "Cannot connect to database"**

**Síntomas:**
```
System.Data.SqlClient.SqlException: A network-related or instance-specific 
error occurred while establishing a connection to SQL Server.
```

**Soluciones:**

```powershell
# 1. Verificar que LocalDB está corriendo
sqllocaldb info

# Si no está, iniciar
sqllocaldb start mssqllocaldb

# 2. Verificar instancia existe
sqllocaldb query

# 3. Probar conexión con sqlcmd
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT 1"

# 4. Ver lista de instancias
sqlcmd -L
```

**Verificar connection string:**

```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=(localdb)\\mssqllocaldb;Database=Northwind;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**Para SQL Server Express:**
```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=.\\SQLEXPRESS;Database=Northwind;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

---

### **Error: "Database Northwind not found"**

**Síntomas:**
```
Microsoft.Data.SqlClient.SqlException: Cannot open database "Northwind" 
requested by the login. The login failed.
```

**Soluciones:**

```sql
-- 1. Conectarse al servidor
-- 2. Verificar que existe la BD
SELECT * FROM sys.databases WHERE name = 'Northwind';

-- 3. Si no existe, crear
CREATE DATABASE Northwind;

-- 4. Si existe pero está vacía, restaurar backup
-- Descargar Northwind script desde:
-- https://github.com/microsoft/sql-server-samples
```

**Alternativa: Usar scaffolding**

```bash
# Generar DbContext desde BD existente
cd src/NorthwindTradersApp.Infrastructure

dotnet ef dbcontext scaffold \
  "Server=(localdb)\mssqllocaldb;Database=Northwind;Trusted_Connection=true;" \
  Microsoft.EntityFrameworkCore.SqlServer \
  -o Persistence \
  -f
```

---

### **Error: "Login failed for user"**

**Síntomas:**
```
Cannot open database "Northwind" requested by the login. The login failed 
for user 'DESKTOP-XXXXX\username'.
```

**Soluciones:**

```powershell
# 1. Usar SQL Server Authentication en lugar de Windows Auth
# Connection string con Usuario/Contraseña
"Server=localhost;Database=Northwind;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"

# 2. Crear usuario en SQL Server
# Abrir SQL Server Management Studio
# Security > Logins > New Login
# - Login name: sa
# - Password: YourPassword
# - Enable checkbox: "User must change password at next login"
```

---

## Build & Compilation Errors

### **Error: "Project file cannot be loaded"**

**Síntomas:**
```
Could not load file or assembly NorthwindTradersApp.Api.csproj
```

**Soluciones:**

```bash
# 1. Limpiar compilación anterior
dotnet clean

# 2. Eliminar carpetas de caché
Remove-Item -Recurse bin/
Remove-Item -Recurse obj/

# 3. Restaurar paquetes
dotnet restore

# 4. Compilar de nuevo
dotnet build
```

---

### **Error: "Package version not found"**

**Síntomas:**
```
error NU1101: Unable to find package Microsoft.EntityFrameworkCore version 10.0.0
```

**Soluciones:**

```bash
# 1. Limpiar caché NuGet
dotnet nuget locals all --clear

# 2. Restaurar
dotnet restore

# 3. Verificar versión disponible
dotnet nuget search Microsoft.EntityFrameworkCore --exact

# 4. Actualizar en .csproj si es necesario
# <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
```

---

### **Error: "The type or namespace does not exist"**

**Síntomas:**
```
CS0246: The type or namespace name 'OrderDto' could not be found
```

**Soluciones:**

```csharp
// 1. Verificar que el archivo existe
// src/NorthwindTradersApp.Domain/Contracts/OrderDto.cs

// 2. Verificar namespace está correcto
namespace NorthwindTradersApp.Domain.Contracts
{
    public sealed class OrderDto { }
}

// 3. En el archivo que lo usa, agregar using
using NorthwindTradersApp.Domain.Contracts;

// 4. Limpiar solución
dotnet clean
dotnet build
```

---

### **Error: "Ambiguous reference"**

**Síntomas:**
```
error CS0104: 'Product' is an ambiguous reference between...
```

**Soluciones:**

```csharp
// 1. Usar nombre completo
NorthwindTradersApp.Domain.NorthwindDbEntities.Product product;

// 2. Usar alias
using ProductEntity = NorthwindTradersApp.Domain.NorthwindDbEntities.Product;
using ProductDto = NorthwindTradersApp.Domain.Contracts.ProductDto;

// 3. Evitar conflictos de nombres en namespaces
```

---

## Runtime Errors

### **Error: "NullReferenceException"**

**Síntomas:**
```
System.NullReferenceException: Object reference not set to an instance of an object.
at NorthwindTradersApp.Application.Services.ProductService.GetAllProducts()
```

**Soluciones:**

```csharp
// ❌ MAL - Sin null check
var products = _repository.GetAllProductsAsync();
foreach (var product in products) // null reference
{
    var name = product.ProductName;
}

// ✅ BIEN - Con null check
var products = await _repository.GetAllProductsAsync();
if (products == null)
    return new List<ProductDto>();

foreach (var product in products)
{
    var name = product?.ProductName ?? string.Empty;
}

// ✅ BIEN - Null coalescing
var name = product?.ProductName ?? "Unknown";

// ✅ BIEN - Guard clause
if (product == null)
    throw new ArgumentNullException(nameof(product));
```

---

### **Error: "InvalidOperationException" en DbContext**

**Síntomas:**
```
System.InvalidOperationException: A second operation was started on this context 
before a previous operation completed.
```

**Soluciones:**

```csharp
// ❌ MAL - Falta await
var products = _context.Products.ToListAsync(); // No await
foreach (var p in products) // Exception aquí
{
}

// ✅ BIEN - Usar await
var products = await _context.Products.ToListAsync();
foreach (var p in products)
{
}

// ✅ BIEN - Si necesitas compilación diferida
var query = _context.Products
    .Where(p => p.UnitPrice > 10)
    .OrderBy(p => p.ProductName);

var results = await query.ToListAsync(); // Aquí se ejecuta
```

---

### **Error: "DbUpdateException"**

**Síntomas:**
```
Microsoft.EntityFrameworkCore.DbUpdateException: 
An error occurred while updating the entries.
See the inner exception for details.
```

**Soluciones:**

```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    // Ver inner exception para más detalles
    Console.WriteLine(ex.InnerException?.Message);
    
    // Posibles causas:
    // 1. Violación de clave foránea
    // 2. Violación de constrains único
    // 3. Valor NULL en columna NOT NULL
    // 4. Tipo de dato incompatible
}
```

---

## API Issues

### **Error: "Port 5000 already in use"**

**Síntomas:**
```
Unhandled exception. System.IO.IOException: Failed to bind to address 
http://[::]:5000: Address already in use.
```

**Soluciones:**

```powershell
# 1. Encontrar qué está usando el puerto
netstat -ano | findstr :5000
# Resultado: TCP    0.0.0.0:5000      0.0.0.0:0        LISTENING       1234

# 2. Matar el proceso
taskkill /PID 1234 /F

# 3. O usar puerto diferente
dotnet run --urls="http://localhost:5001"

# 4. O cambiar en launchSettings.json
# "applicationUrl": "http://localhost:5001"
```

---

### **Error: "404 Not Found" en endpoint válido**

**Síntomas:**
```
GET http://localhost:5000/api/products
Response: 404 Not Found
```

**Soluciones:**

```csharp
// 1. Verificar que endpoint está mapeado en Program.cs
app.MapGet("/api/products", GetAllProducts)
   .WithName("GetAllProducts");

// 2. Verificar naming correcto
// /api/products (no /api/product)

// 3. Verificar que MapGet está después de middleware
app.UseRouting();
app.MapGet("/api/products", GetAllProducts); // ✅ Después de UseRouting

// 4. Verificar método que maneja el endpoint existe
async Task<IResult> GetAllProducts(IProductsService service)
{
    var products = await service.GetAllProductsAsync();
    return Results.Ok(products);
}
```

---

### **Error: "405 Method Not Allowed"**

**Síntomas:**
```
POST http://localhost:5000/api/products
Response: 405 Method Not Allowed
```

**Soluciones:**

```csharp
// 1. Verificar que el método HTTP es correcto
// POST para crear, PUT para actualizar, DELETE para eliminar

// 2. Verificar que está mapeado
app.MapPost("/api/products", CreateProduct); // ✅ Correcto

// 3. Verificar que no hay conflicto con ruta
// Si existe MapGet("/api/products/{id}"), 
// MapPost debe ser en ruta diferente o el handler debe estar en MapPost

// 4. Asegurar que CORS está configurado si es cross-origin
app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());
```

---

### **Error: "400 Bad Request"**

**Síntomas:**
```
POST http://localhost:5000/api/products
Body: { "productName": "Chai" }
Response: 400 Bad Request
```

**Soluciones:**

```csharp
// 1. Verificar JSON es válido
// Usar herramientas como jsonlint.com

// 2. Verificar que headers incluyen Content-Type
Headers:
  Content-Type: application/json

// 3. Verificar que propiedades coinciden con DTO
public sealed class CreateProductRequest
{
    public string ProductName { get; set; } // Coincide con JSON
    public decimal UnitPrice { get; set; }
}

// 4. Agregar validación explícita en endpoint
app.MapPost("/api/products", async (CreateProductRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.ProductName))
        return Results.BadRequest(new { error = "ProductName is required" });
    
    if (request.UnitPrice <= 0)
        return Results.BadRequest(new { error = "UnitPrice must be positive" });
    
    // ...
});
```

---

### **Error: "500 Internal Server Error"**

**Síntomas:**
```
Response: 500 Internal Server Error
No detalles del error
```

**Soluciones:**

```csharp
// 1. Habilitar detailed error messages en Development
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 2. Agregar logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 3. Agregar exception handler global
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = exception?.Message,
            stackTrace = exception?.StackTrace // Solo en Development
        });
    });
});

// 4. Revisar logs en Output window
// Debug > Windows > Output
```

---

## Performance Issues

### **Problema: "Queries are slow"**

**Soluciones:**

```csharp
// ❌ MAL - N+1 Query Problem
var orders = await _context.Orders.ToListAsync();
foreach (var order in orders)
{
    // Esto causa una query POR cada order
    var customer = await _context.Customers
        .FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId);
}

// ✅ BIEN - Use Include
var orders = await _context.Orders
    .Include(o => o.Customer)
    .Include(o => o.Employee)
    .Include(o => o.OrderDetails)
    .ToListAsync();

// ✅ BIEN - Use Select para projeccion
var productNames = await _context.Products
    .Select(p => new { p.ProductId, p.ProductName })
    .ToListAsync();

// ✅ BIEN - Use ExecuteUpdateAsync para bulk updates
await _context.Products
    .Where(p => p.Discontinued)
    .ExecuteUpdateAsync(s =>
        s.SetProperty(p => p.UnitsInStock, 0));
```

---

### **Problema: "Memory usage is high"**

**Soluciones:**

```csharp
// ❌ MAL - Cargar todo a memoria
var products = _context.Products.ToList(); // 1000+ registros en memoria

// ✅ BIEN - Usar paginación
var pageSize = 20;
var page = 1;

var products = await _context.Products
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// ✅ BIEN - Usar cursor-based pagination para datasets grandes
public async Task<List<ProductDto>> GetProductsPaginatedAsync(
    int lastProductId,
    int pageSize = 20)
{
    return await _context.Products
        .Where(p => p.ProductId > lastProductId)
        .OrderBy(p => p.ProductId)
        .Take(pageSize)
        .ToListAsync();
}
```

---

## Common Exceptions

### **KeyNotFoundException**

```csharp
try
{
    await _orderRepository.DeleteOrderAsync(99999);
}
catch (KeyNotFoundException ex)
{
    // Order 99999 not found
    return Results.NotFound(new { error = ex.Message });
}
```

**Solución:** Verifica que la entidad existe antes de operaciones

---

### **ValidationException**

```csharp
try
{
    var result = await _validator.ValidateAsync(productDto);
    if (!result.IsValid)
    {
        var errors = string.Join(", ", 
            result.Errors.Select(e => e.ErrorMessage));
        throw new ValidationException(errors);
    }
}
catch (ValidationException ex)
{
    return Results.BadRequest(new { error = ex.Message });
}
```

**Solución:** Implementa FluentValidation para validación robusta

---

### **DbConcurrencyException**

```csharp
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex)
{
    // Alguien más modificó la entidad
    // Recargar y reintentar o notificar al usuario
    var entity = ex.Entries.First();
    var databaseValues = await entity.GetDatabaseValuesAsync();
    var proposedValues = entity.CurrentValues;
    
    // Resolver conflicto...
}
```

**Solución:** Implementa estrategia de concurrencia (ej: timestamps)

---

## Debugging Tips

### **1. Usar Debug Breakpoints**

```csharp
// En VS Code o Visual Studio
// Hacer click en la línea número para agregar breakpoint
// F5 para iniciar debug
// F10 para siguiente línea
// F11 para entrar en función

public async Task<ProductDto> GetProductAsync(int id)
{
    // Breakpoint aquí para inspeccionar
    var product = await _repository.GetByIdAsync(id);
    
    return product;
}
```

---

### **2. Logging Detallado**

```csharp
// Program.cs
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// En servicios
private readonly ILogger<ProductService> _logger;

public ProductService(
    IProductsRepository repository,
    ILogger<ProductService> logger)
{
    _repository = repository;
    _logger = logger;
}

public async Task<ProductDto> GetProductAsync(int id)
{
    _logger.LogInformation("Getting product with ID: {ProductId}", id);
    
    try
    {
        var product = await _repository.GetByIdAsync(id);
        _logger.LogInformation("Product found: {ProductName}", product?.ProductName);
        return product;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
        throw;
    }
}
```

---

### **3. Inspeccionar SQL Queries**

```csharp
// Program.cs
builder
    .Logging
    .AddConsole()
    .AddFilter(level => level >= LogLevel.Information);

builder
    .Services
    .AddDbContext<NorthwindDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
        
        // Mostrar SQL en console
        options.LogTo(
            Console.WriteLine,
            new[] { DbLoggerCategory.Database.Command.Name },
            LogLevel.Information);
    });

// Verás las queries en el output
// info: Microsoft.EntityFrameworkCore.Database.Command[20101]
// Executed DbCommand (5ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
// SELECT [p].[ProductId], [p].[ProductName] FROM [dbo].[Products] AS [p]
```

---

### **4. Inspeccionar DbContext State**

```csharp
var product = new Product { ProductId = 1, ProductName = "Chai" };
_context.Products.Add(product);

Console.WriteLine($"State: {_context.Entry(product).State}");
// Output: State: Added

await _context.SaveChangesAsync();

Console.WriteLine($"State after save: {_context.Entry(product).State}");
// Output: State after save: Unchanged
```

---

### **5. Health Checks**

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<NorthwindDbContext>();

app.MapHealthChecks("/health");

// Test
// GET http://localhost:5000/health
// Response: Healthy
```

---

## Recursos Útiles

- [Microsoft Docs - Troubleshooting](https://docs.microsoft.com/en-us/ef/core/miscellaneous/logging)
- [Entity Framework Core - Common Errors](https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/)
- [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [DbMonitor - Query Performance](https://www.redgate.com/products/sql-development/)

---

## Checklist de Troubleshooting

Cuando tengas un problema, usa este checklist:

- [ ] ¿Hay mensajes de error específicos? (Búscalos en la documentación)
- [ ] ¿El ambiente está correctamente configurado? (Verificar connection string)
- [ ] ¿Todas las dependencias están instaladas? (dotnet restore)
- [ ] ¿La solución compila sin errores? (dotnet build)
- [ ] ¿La BD está disponible? (sqlcmd o SQL Management Studio)
- [ ] ¿Los logs muestran información útil? (Habilitar logging)
- [ ] ¿Hay queries N+1? (Revisar Include())</
- [ ] ¿Las excepciones internas dan más contexto? (Revisar InnerException)
