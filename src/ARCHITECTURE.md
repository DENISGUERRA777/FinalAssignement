# NorthwindTradersApp Backend - Arquitectura

## 📋 Descripción General

NorthwindTradersApp es una aplicación de backend construida con **.NET 10** siguiendo patrones de **Clean Architecture** y **SOLID principles**. El sistema gestiona un catálogo de productos, pedidos de clientes, y empleados de una empresa de comercio.

---

## 🏗️ Estructura de Capas

### **Arquitectura en Capas**

```
┌─────────────────────────────────────────────┐
│  NorthwindTradersApp.Api (Presentation)     │
│  - Controladores REST                       │
│  - Validación HTTP                          │
│  - Swagger/OpenAPI                          │
└────────────────────┬────────────────────────┘
                     │
┌────────────────────▼────────────────────────┐
│  NorthwindTradersApp.Application (Business) │
│  - Lógica de negocio                        │
│  - Servicios                                │
│  - Orquestación                             │
└────────────────────┬────────────────────────┘
                     │
┌────────────────────▼────────────────────────┐
│  NorthwindTradersApp.Domain (Entities)      │
│  - Entidades (Products, Orders, etc)        │
│  - DTOs (Data Transfer Objects)             │
│  - Interfaces de Repositorios               │
└────────────────────┬────────────────────────┘
                     │
┌────────────────────▼────────────────────────┐
│  NorthwindTradersApp.Infrastructure (Data)  │
│  - Entity Framework Core                    │
│  - Repositorios                             │
│  - DbContext (NorthwindDbContext)           │
│  - Acceso a Base de Datos                   │
└─────────────────────────────────────────────┘
                     │
        ┌────────────▼────────────┐
        │   SQL Server DB         │
        │   (Northwind Database)  │
        └─────────────────────────┘
```

### **Responsabilidades por Capa**

| Capa | Proyecto | Responsabilidades |
|------|----------|------------------|
| **API** | NorthwindTradersApp.Api | Manejo de requests HTTP, endpoints REST, validación de entrada, respuestas JSON |
| **Application** | NorthwindTradersApp.Application | Servicios de negocio, orquestación, lógica de validación |
| **Domain** | NorthwindTradersApp.Domain | Definición de entidades, DTOs, contratos de repositorios |
| **Infrastructure** | NorthwindTradersApp.Infrastructure | Persistencia de datos, EF Core, acceso a BD |

---

## 📦 Componentes Principales

### **1. Entidades de Dominio (11 Tablas)**

```
Product ─────┐
             ├─→ Order ──→ OrderDetail ──→ Customer
Category ────┤    ↓
             └─→ Shipper
    
Employee ──→ EmployeeTerritory ──→ Territory
  ↓
Region

Supplier → Product
```

**Entidades Clave:**

- **Order**: Pedidos de clientes (OrderId, CustomerId, EmployeeId, OrderDate)
- **OrderDetail**: Detalles de líneas en pedidos (OrderId, ProductId, UnitPrice, Quantity)
- **Product**: Productos disponibles (ProductId, ProductName, UnitPrice)
- **Customer**: Clientes (CustomerId, CompanyName, ContactName)
- **Employee**: Empleados (EmployeeId, FirstName, LastName, Title)
- **Category**: Categorías de productos
- **Supplier**: Proveedores
- **Shipper**: Transportistas

---

## 🔄 Patrón de Flujo de Datos

### **Ejemplo: Crear un Pedido (Order)**

```
1. Cliente HTTP
   │
   ├─→ POST /api/orders
   │   └─ Body: OrderDto { CustomerId, EmployeeId, OrderDetails[] }
   │
2. API Layer (Program.cs)
   │
   ├─→ ValidaRequest HTTP
   ├─→ Llama OrdersService.CreateOrderAsync(orderDto)
   │
3. Application Layer (OrdersService)
   │
   ├─→ Aplica lógica de negocio
   ├─→ Llama OrdersRepository.CreateOrderAsync(orderDto)
   │
4. Infrastructure Layer (OrdersRepository)
   │
   ├─→ Mapea OrderDto → Order Entity
   ├─→ Crea Order en DbContext
   ├─→ SaveChangesAsync() → Obtiene OrderId
   ├─→ Crea OrderDetail entities
   ├─→ SaveChangesAsync() → Persiste detalles
   ├─→ Retorna OrderDto completo
   │
5. API Response
   └─→ HTTP 201 Created + Location header
```

---

## 🔧 Patrones de Diseño Implementados

### **1. Repository Pattern**
- **Interfaz**: `IOrdersRepository`, `IProductsRepository`, `ICustomersRepository`
- **Implementación**: Clases `sealed` para mejor rendimiento
- **Beneficio**: Abstracción del acceso a datos

### **2. Dependency Injection (DI)**
```csharp
// Application Services
services.AddScoped<IOrdersService, OrdersService>();
services.AddScoped<IProductsService, ProductService>();
services.AddScoped<ICustomersService, CustomersService>();

// Infrastructure Repositories
services.AddScoped<IOrdersRepository, OrdersRepository>();
services.AddScoped<IProductsRepository, ProductsRepository>();
services.AddScoped<ICustomersRepository, CustomersRepository>();
```

### **3. Data Transfer Objects (DTOs)**
- **Desacoplamiento**: API no expone entidades internas directamente
- **Ejemplo**: `OrderDto` incluye nested `List<OrderDetailDto>`
- **Ventaja**: Cambios en BD no afectan contrato API

### **4. Entity Framework Core - Async/Await**
```csharp
// Todas las operaciones son asincrónicas
public async Task<List<OrderDto>> GetAllOrdersAsync()
{
    var orders = await _context.Orders
        .Include(o => o.OrderDetails)
        .ToListAsync();
    
    return orders.Select(MapToDto).ToList();
}
```

### **5. LINQ para Queries Avanzadas**
```csharp
// Búsqueda con JOIN
var orders = await _context.Orders
    .Join(_context.Customers, 
        o => o.CustomerId, 
        c => c.CustomerId,
        (o, c) => new { Order = o, Customer = c })
    .Where(x => x.Order.OrderId == orderId)
    .ToListAsync();
```

### **6. Error Handling**
```csharp
public async Task<bool> DeleteOrderAsync(int orderId)
{
    var order = await _context.Orders
        .Include(o => o.OrderDetails)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);
    
    if (order == null)
        throw new KeyNotFoundException($"Order {orderId} not found");
    
    _context.Orders.Remove(order);
    await _context.SaveChangesAsync();
    return true;
}
```

---

## 🗄️ Base de Datos

### **Motor**: SQL Server
### **Nombre**: NorthwindDatabase
### **Configuración**: appsettings.json

```json
{
  "ConnectionStrings": {
    "NorthwindDatabase": "Server=.;Database=Northwind;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### **Entity Framework Core Setup**

```csharp
services.AddDbContext<NorthwindDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("NorthwindDatabase")
    )
);
```

---

## 🔑 Características Principales

| Característica | Detalles |
|---|---|
| **Async/Await** | Todas las operaciones I/O son asincrónicas |
| **Paginación** | Soportada en todos los endpoints GET |
| **Búsqueda** | Endpoints `/search` para filtrado específico |
| **CRUD Completo** | Create, Read, Update, Delete para cada recurso |
| **Validación** | En API layer con FluentValidation (si aplica) |
| **Documentación** | Swagger/OpenAPI integrado |
| **Transaction Safety** | Múltiples SaveChangesAsync() para datos relacionados |
| **Sealed Classes** | Optimización de rendimiento JIT |

---

## 📊 Diagrama de Entidades (ER)

```
┌──────────────────┐
│   Categories     │
│ ────────────────│
│ CategoryId (PK)  │
│ CategoryName     │
│ Description      │
└────────┬─────────┘
         │ 1:N
         │
┌────────▼──────────┐         ┌──────────────────┐
│   Products        │────────→│   Suppliers      │
│ ────────────────  │ N:1     │ ────────────────│
│ ProductId (PK)    │         │ SupplierId (PK)  │
│ ProductName       │         │ Company          │
│ UnitPrice         │         └──────────────────┘
│ UnitsInStock      │
│ CategoryId (FK)   │
│ SupplierId (FK)   │
└────────┬──────────┘
         │ N:M
         │
┌────────▼──────────────┐
│  OrderDetails        │
│ ────────────────────│
│ OrderId (FK/PK)      │
│ ProductId (FK/PK)    │
│ UnitPrice            │
│ Quantity             │
│ Discount             │
└──────────────────────┘
         ▲
         │ N:1
         │
    ┌────┴──────────────────┐
    │      Orders           │
    │ ────────────────────  │
    │ OrderId (PK)          │
    │ CustomerId (FK)       │
    │ EmployeeId (FK)       │
    │ OrderDate             │
    │ ShipVia (FK)          │
    │ Freight               │
    └────┬──────────────────┘
         │          │
         │ N:1      │ N:1
         │          │
    ┌────▼──┐   ┌──▼───────────┐
    │        │   │              │
┌───┴──────┐ │ ┌┴────────────┐  │
│Customers │─┘ │  Employees  │  │
│──────────│    │ ────────────│  │
│CustomerId    │ EmployeeId  │  │
│CompanyName   │ FirstName   │  │
│ContactName   │ LastName    │  │
│Address       │ Title       │  │
│City          │ ReportsTo   │  │
└───────────┘  └──────┬──────┘  │
                       │         │
                       │ Self:1:N
                       │
                     ┌─▼──────────────┐
                     │  Shippers      │
                     │ ────────────── │
                     │ ShipperId (PK) │
                     │ CompanyName    │
                     └────────────────┘
```

---

## 🚀 Stack Tecnológico

| Tecnología | Versión | Propósito |
|---|---|---|
| **.NET** | 10.0 | Framework principal |
| **Entity Framework Core** | Latest | ORM para acceso a datos |
| **SQL Server** | 2019+ | Base de datos relacional |
| **Swagger** | 6.x | Documentación API |
| **LINQ** | 10.0 | Queries de datos |

---

## 📝 Convenciones de Código

1. **Clases Sealed**: Para mejor rendimiento JIT
2. **Async Task**: Todas las operaciones I/O
3. **DTOs**: Separación entre modelos internos y API
4. **Null Coalescing**: Manejo seguro de nulos
5. **String Interpolation**: `$"Order {orderId} not found"`
6. **LINQ Queries**: En lugar de SQL directo

---

## 🔍 Referencias Rápidas

- **Archivo Principal API**: [Program.cs](NorthwindTradersApp.Api/Program.cs)
- **HTTP Requests**: [NorthwindTradersApp.Api.http](NorthwindTradersApp.Api/NorthwindTradersApp.Api.http)
- **DbContext**: [NorthwindDbContext.cs](NorthwindTradersApp.Infrastructure/Persistence/NorthwindDbContext.cs)
- **Servicios**: [Services/](NorthwindTradersApp.Application/Services/)
- **Repositorios**: [Repositories/](NorthwindTradersApp.Infrastructure/Repositories/)

---

## 📚 Documentación Relacionada

- [API Endpoints Documentation](API_ENDPOINTS.md)
- [Database Schema](DATABASE_SCHEMA.md)
- [Setup & Installation](SETUP_AND_INSTALLATION.md)
- [Development Guide](DEVELOPMENT_GUIDE.md)
- [Troubleshooting Guide](TROUBLESHOOTING.md)
