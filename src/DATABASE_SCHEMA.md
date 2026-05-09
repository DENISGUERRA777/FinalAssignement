# 🗄️ NorthwindTradersApp - Database Schema

## Base de Datos: Northwind

**Motor**: SQL Server 2019+  
**Nombre**: `Northwind`  
**Tipo**: Relacional (OLTP)

---

## 📋 Tabla de Contenidos

1. [Tablas](#tablas)
2. [Claves Primarias](#claves-primarias)
3. [Claves Foráneas](#claves-foráneas)
4. [Índices](#índices)
5. [Relaciones](#relaciones)
6. [Secuencias de Datos](#secuencias-de-datos)

---

## Tablas

### **1. Customers** - Clientes

Tabla de clientes que realizan pedidos.

| Columna | Tipo | Null | PK | Descripción |
|---------|------|------|----|----|
| CustomerId | nchar(5) | NO | ✓ | Identificador único del cliente |
| CompanyName | nvarchar(40) | NO | | Nombre de la empresa |
| ContactName | nvarchar(30) | YES | | Nombre del contacto principal |
| ContactTitle | nvarchar(30) | YES | | Título del contacto |
| Address | nvarchar(60) | YES | | Dirección |
| City | nvarchar(15) | YES | | Ciudad |
| Region | nvarchar(15) | YES | | Región/Estado |
| PostalCode | nvarchar(10) | YES | | Código postal |
| Country | nvarchar(15) | YES | | País |
| Phone | nvarchar(24) | YES | | Teléfono |
| Fax | nvarchar(24) | YES | | Fax |

**Índices:**
- `PK_Customers` (CustomerId)

**Ejemplo de Datos:**
```
CustomerId | CompanyName | ContactName | City | Country | Phone
-----------|-------------|-------------|------|---------|------
ALFKI      | Alfreds Futterkiste | Maria Anders | Berlin | Germany | 030-0074321
ANATR      | Ana Trujillo | Ana Trujillo | México D.F. | Mexico | (5) 555-4729
ANTON      | Antonio Moreno | Antonio Moreno | México D.F. | Mexico | (5) 555-3932
```

---

### **2. Products** - Productos

Tabla de productos disponibles para venta.

| Columna | Tipo | Null | PK | FK | Descripción |
|---------|------|------|----|----|-----------|
| ProductId | int | NO | ✓ | | Identificador único del producto |
| ProductName | nvarchar(40) | NO | | | Nombre del producto |
| SupplierId | int | YES | | ✓ | ID del proveedor (Suppliers) |
| CategoryId | int | YES | | ✓ | ID de la categoría (Categories) |
| QuantityPerUnit | nvarchar(20) | YES | | | Cantidad por unidad (ej: "10 boxes") |
| UnitPrice | money | YES | | | Precio unitario |
| UnitsInStock | smallint | YES | | | Cantidad en inventario |
| UnitsOnOrder | smallint | YES | | | Cantidad en pedidos pendientes |
| ReorderLevel | smallint | YES | | | Nivel mínimo de reorden |
| Discontinued | bit | NO | | | Indica si el producto está descontinuado |

**Índices:**
- `PK_Products` (ProductId)
- `FK_Products_Categories` (CategoryId)
- `FK_Products_Suppliers` (SupplierId)

**Ejemplo de Datos:**
```
ProductId | ProductName | CategoryId | UnitPrice | UnitsInStock | Discontinued
-----------|-------------|-----------|-----------|-------------|-----------|
1          | Chai | 1 | 18.00 | 39 | 0
2          | Chang | 1 | 19.00 | 17 | 0
3          | Aniseed Syrup | 2 | 10.00 | 13 | 0
```

---

### **3. Categories** - Categorías

Tabla de categorías de productos.

| Columna | Tipo | Null | PK | Descripción |
|---------|------|------|----|----|
| CategoryId | int | NO | ✓ | Identificador único de la categoría |
| CategoryName | nvarchar(15) | NO | | Nombre de la categoría |
| Description | ntext | YES | | Descripción detallada |
| Picture | image | YES | | Imagen de la categoría (binario) |

**Índices:**
- `PK_Categories` (CategoryId)

**Ejemplo de Datos:**
```
CategoryId | CategoryName | Description
-----------|-------------|-----------|
1 | Beverages | Soft drinks, coffees, teas, beers, and ales
2 | Condiments | Sweet and savory sauces, relishes, spreads, and seasonings
3 | Confections | Desserts, candies, and sweet breads
```

---

### **4. Suppliers** - Proveedores

Tabla de proveedores de productos.

| Columna | Tipo | Null | PK | Descripción |
|---------|------|------|----|----|
| SupplierId | int | NO | ✓ | Identificador único del proveedor |
| CompanyName | nvarchar(40) | NO | | Nombre de la empresa proveedora |
| ContactName | nvarchar(30) | YES | | Nombre del contacto |
| ContactTitle | nvarchar(30) | YES | | Título del contacto |
| Address | nvarchar(60) | YES | | Dirección |
| City | nvarchar(15) | YES | | Ciudad |
| Region | nvarchar(15) | YES | | Región/Estado |
| PostalCode | nvarchar(10) | YES | | Código postal |
| Country | nvarchar(15) | YES | | País |
| Phone | nvarchar(24) | YES | | Teléfono |
| Fax | nvarchar(24) | YES | | Fax |

---

### **5. Orders** - Pedidos

Tabla principal de pedidos.

| Columna | Tipo | Null | PK | FK | Descripción |
|---------|------|------|----|----|-----------|
| OrderId | int | NO | ✓ | | Identificador único del pedido |
| CustomerId | nchar(5) | YES | | ✓ | ID del cliente (Customers) |
| EmployeeId | int | YES | | ✓ | ID del empleado (Employees) |
| OrderDate | datetime | YES | | | Fecha del pedido |
| RequiredDate | datetime | YES | | | Fecha requerida de entrega |
| ShippedDate | datetime | YES | | | Fecha de envío actual |
| ShipVia | int | YES | | ✓ | ID del transportista (Shippers) |
| Freight | money | YES | | | Costo de envío |
| ShipName | nvarchar(40) | YES | | | Nombre del destino |
| ShipAddress | nvarchar(60) | YES | | | Dirección de envío |
| ShipCity | nvarchar(15) | YES | | | Ciudad de envío |
| ShipRegion | nvarchar(15) | YES | | | Región de envío |
| ShipPostalCode | nvarchar(10) | YES | | | Código postal de envío |
| ShipCountry | nvarchar(15) | YES | | | País de envío |

**Índices:**
- `PK_Orders` (OrderId)
- `FK_Orders_Customers` (CustomerId)
- `FK_Orders_Employees` (EmployeeId)
- `FK_Orders_Shippers` (ShipVia)

**Ejemplo de Datos:**
```
OrderId | CustomerId | EmployeeId | OrderDate | Freight | ShipVia
--------|-----------|-----------|-----------|---------|--------
10248 | VINET | 5 | 2023-07-04 | 32.38 | 3
10249 | TOMSP | 6 | 2023-07-05 | 11.61 | 1
10250 | HANAR | 4 | 2023-07-08 | 65.83 | 2
```

---

### **6. OrderDetails** - Detalles de Pedidos

Tabla de línea detallada de cada pedido.

| Columna | Tipo | Null | PK | FK | Descripción |
|---------|------|------|----|----|-----------|
| OrderId | int | NO | ✓ | ✓ | ID del pedido (Orders) |
| ProductId | int | NO | ✓ | ✓ | ID del producto (Products) |
| UnitPrice | money | NO | | | Precio unitario al momento del pedido |
| Quantity | smallint | NO | | | Cantidad pedida |
| Discount | real | NO | | | Descuento aplicado (0.0 - 1.0) |

**Índices:**
- `PK_OrderDetails` (OrderId, ProductId)
- `FK_OrderDetails_Orders` (OrderId)
- `FK_OrderDetails_Products` (ProductId)

**Ejemplo de Datos:**
```
OrderId | ProductId | UnitPrice | Quantity | Discount
--------|-----------|-----------|----------|----------
10248 | 11 | 14.00 | 12 | 0.0
10248 | 42 | 9.80 | 10 | 0.0
10248 | 72 | 34.80 | 5 | 0.0
10249 | 14 | 23.25 | 9 | 0.0
```

---

### **7. Employees** - Empleados

Tabla de empleados.

| Columna | Tipo | Null | PK | FK | Descripción |
|---------|------|------|----|----|-----------|
| EmployeeId | int | NO | ✓ | | Identificador único del empleado |
| LastName | nvarchar(20) | NO | | | Apellido |
| FirstName | nvarchar(10) | NO | | | Nombre |
| Title | nvarchar(30) | YES | | | Título del puesto |
| TitleOfCourtesy | nvarchar(25) | YES | | | Tratamiento (Sr., Sra., Dr.) |
| BirthDate | datetime | YES | | | Fecha de nacimiento |
| HireDate | datetime | YES | | | Fecha de contratación |
| Address | nvarchar(60) | YES | | | Dirección |
| City | nvarchar(15) | YES | | | Ciudad |
| Region | nvarchar(15) | YES | | | Región/Estado |
| PostalCode | nvarchar(10) | YES | | | Código postal |
| Country | nvarchar(15) | YES | | | País |
| HomePhone | nvarchar(24) | YES | | | Teléfono personal |
| Extension | nvarchar(4) | YES | | | Extensión telefónica |
| Photo | image | YES | | | Foto del empleado (binario) |
| Notes | ntext | YES | | | Notas adicionales |
| ReportsTo | int | YES | | ✓ | ID del supervisor (auto-referencia) |

**Índices:**
- `PK_Employees` (EmployeeId)
- `FK_Employees_Employees` (ReportsTo - Jerarquía)

---

### **8. Shippers** - Transportistas

Tabla de empresas transportistas.

| Columna | Tipo | Null | PK | Descripción |
|---------|------|------|----|----|
| ShipperId | int | NO | ✓ | Identificador único del transportista |
| CompanyName | nvarchar(40) | NO | | Nombre de la empresa |
| Phone | nvarchar(24) | YES | | Teléfono |

**Índices:**
- `PK_Shippers` (ShipperId)

**Ejemplo de Datos:**
```
ShipperId | CompanyName | Phone
-----------|-------------|---------|
1 | Speedy Express | (503) 555-9831
2 | United Package | (503) 555-3199
3 | Federal Shipping | (503) 555-9931
```

---

### **9. Region** - Regiones

Tabla de regiones.

| Columna | Tipo | Null | PK | Descripción |
|---------|------|------|----|----|
| RegionId | int | NO | ✓ | Identificador único de región |
| RegionDescription | nchar(50) | NO | | Descripción de la región |

---

### **10. Territories** - Territorios

Tabla de territorios de ventas.

| Columna | Tipo | Null | PK | FK | Descripción |
|---------|------|------|----|----|-----------|
| TerritoryId | nvarchar(20) | NO | ✓ | | Identificador único del territorio |
| TerritoryDescription | nchar(50) | NO | | | Descripción del territorio |
| RegionId | int | NO | | ✓ | ID de la región (Region) |

---

### **11. EmployeeTerritories** - Empleado-Territorio (Asignaciones)

Tabla de relación muchos-a-muchos entre Empleados y Territorios.

| Columna | Tipo | Null | PK | FK | Descripción |
|---------|------|------|----|----|-----------|
| EmployeeId | int | NO | ✓ | ✓ | ID del empleado (Employees) |
| TerritoryId | nvarchar(20) | NO | ✓ | ✓ | ID del territorio (Territories) |

**Índices:**
- `PK_EmployeeTerritories` (EmployeeId, TerritoryId)

---

## Claves Primarias

| Tabla | Clave Primaria | Tipo |
|-------|---|---|
| Customers | CustomerId | Identificador de texto (nchar(5)) |
| Products | ProductId | Secuencia automática (int) |
| Categories | CategoryId | Secuencia automática (int) |
| Suppliers | SupplierId | Secuencia automática (int) |
| Orders | OrderId | Secuencia automática (int) |
| OrderDetails | (OrderId, ProductId) | **Clave Compuesta** |
| Employees | EmployeeId | Secuencia automática (int) |
| Shippers | ShipperId | Secuencia automática (int) |
| Region | RegionId | Secuencia automática (int) |
| Territories | TerritoryId | Identificador de texto (nvarchar(20)) |
| EmployeeTerritories | (EmployeeId, TerritoryId) | **Clave Compuesta** |

---

## Claves Foráneas

| De Tabla | Columna | A Tabla | Columna | Acción |
|----------|---------|---------|---------|--------|
| Products | CategoryId | Categories | CategoryId | CASCADE |
| Products | SupplierId | Suppliers | SupplierId | NO ACTION |
| Orders | CustomerId | Customers | CustomerId | NO ACTION |
| Orders | EmployeeId | Employees | EmployeeId | NO ACTION |
| Orders | ShipVia | Shippers | ShipperId | NO ACTION |
| OrderDetails | OrderId | Orders | OrderId | CASCADE |
| OrderDetails | ProductId | Products | ProductId | NO ACTION |
| Employees | ReportsTo | Employees | EmployeeId | NO ACTION |
| EmployeeTerritories | EmployeeId | Employees | EmployeeId | CASCADE |
| EmployeeTerritories | TerritoryId | Territories | TerritoryId | CASCADE |
| Territories | RegionId | Region | RegionId | NO ACTION |

---

## Índices

### **Índices de Búsqueda Rápida**

```sql
-- Búsquedas por CustomerName en Orders (JOIN)
CREATE INDEX IX_Customers_CompanyName 
ON Customers(CompanyName);

-- Búsquedas por ProductName
CREATE INDEX IX_Products_ProductName 
ON Products(ProductName);

-- Búsquedas por OrderDate
CREATE INDEX IX_Orders_OrderDate 
ON Orders(OrderDate);

-- Búsquedas por EmployeeId
CREATE INDEX IX_Orders_EmployeeId 
ON Orders(EmployeeId);
```

---

## Relaciones

### **Diagrama de Entidades**

```
┌─────────────────────┐
│     Categories      │
│ CategoryId (PK)     │
└────────┬────────────┘
         │ 1:N
         │
┌────────▼──────────────────┐
│      Products             │
│ ProductId (PK)            │
│ CategoryId (FK)           │ ───→ CategoryName
│ SupplierId (FK)           │
└────────┬──────────────────┘
         │ N:M (en OrderDetails)
         │
┌────────▼───────────────────────┐
│     OrderDetails               │
│ (OrderId, ProductId) (PK)      │
│ OrderId (FK)                   │
│ ProductId (FK)                 │
└────────▲───────────────────────┘
         │ N:1
         │
┌────────┴─────────────────────┐
│        Orders               │
│ OrderId (PK)                │
│ CustomerId (FK)             │
│ EmployeeId (FK)             │
│ ShipVia (FK)                │
└────┬──────┬──────────────┬───┘
     │      │              │
   1:N    1:N            1:N
     │      │              │
┌────▼───┐┌─▼─────────────┐┌──▼──────────┐
│Customers││  Employees   ││  Shippers   │
│Customer ││Employee (PK) ││Shipper (PK) │
│  Id(PK) ││ReportsTo(FK) ││              │
└─────────┘└──┬───────────┘└─────────────┘
              │ Self-Ref
              │
          (Jerarquía)

┌──────────────────────┐
│      Territories      │
│ TerritoryId (PK)     │
│ RegionId (FK)        │
└──────┬───────────────┘
       │
       │ N:M (en EmployeeTerritories)
       │
┌──────▼─────────────────────────┐
│   EmployeeTerritories          │
│ (EmployeeId, TerritoryId) (PK) │
│ EmployeeId (FK)                │
│ TerritoryId (FK)               │
└────────────────────────────────┘
```

---

## Secuencias de Datos

### **Insertar Nuevo Pedido**

```
1. INSERT INTO Orders
   (CustomerId, EmployeeId, OrderDate, ShipVia, Freight)
   VALUES ('ALFKI', 1, GETDATE(), 1, 50.00)
   → OrderId = 11078 (auto-generado)

2. INSERT INTO OrderDetails
   (OrderId, ProductId, UnitPrice, Quantity, Discount)
   VALUES (11078, 1, 18.00, 10, 0.0)

3. INSERT INTO OrderDetails
   (OrderId, ProductId, UnitPrice, Quantity, Discount)
   VALUES (11078, 2, 19.00, 5, 0.1)
```

### **Búsqueda de Pedidos por Cliente**

```sql
SELECT o.*, c.CompanyName, e.FirstName
FROM Orders o
JOIN Customers c ON o.CustomerId = c.CustomerId
JOIN Employees e ON o.EmployeeId = e.EmployeeId
WHERE c.CompanyName LIKE '%Alfreds%'
```

### **Detalles Completos de Pedido**

```sql
SELECT 
    o.OrderId,
    o.OrderDate,
    c.CompanyName as CustomerName,
    e.FirstName + ' ' + e.LastName as EmployeeName,
    s.CompanyName as ShipperName,
    od.ProductId,
    p.ProductName,
    od.UnitPrice,
    od.Quantity,
    od.Discount,
    (od.UnitPrice * od.Quantity * (1 - od.Discount)) as LineTotal
FROM Orders o
JOIN Customers c ON o.CustomerId = c.CustomerId
JOIN Employees e ON o.EmployeeId = e.EmployeeId
JOIN Shippers s ON o.ShipVia = s.ShipperId
JOIN OrderDetails od ON o.OrderId = od.OrderId
JOIN Products p ON od.ProductId = p.ProductId
WHERE o.OrderId = 10248
```

---

## Tipología de Datos

| Tipo SQL Server | .NET C# | Descripción | Rango/Tamaño |
|---|---|---|---|
| `int` | `int` | Entero de 32 bits | -2,147,483,648 a 2,147,483,647 |
| `smallint` | `short` | Entero de 16 bits | -32,768 a 32,767 |
| `money` | `decimal` | Dinero de 8 bytes | Precisión de 4 decimales |
| `datetime` | `DateTime` | Fecha y hora | 01/01/1753 a 31/12/9999 |
| `nvarchar(n)` | `string` | Texto variable Unicode | Hasta n caracteres |
| `nchar(n)` | `string` | Texto fijo Unicode | Exactamente n caracteres |
| `ntext` | `string` | Texto largo Unicode | Hasta 2GB |
| `image` | `byte[]` | Datos binarios | Hasta 2GB |
| `bit` | `bool` | Booleano (0 o 1) | 0 (false) o 1 (true) |
| `real` | `float` | Punto flotante de 32 bits | ±1.18e-38 a ±3.40e+38 |

---

## Notas Importantes

1. **Cascada en Eliminaciones**: Las OrderDetails se eliminan automáticamente al eliminar un Order
2. **Claves Compuestas**: OrderDetails y EmployeeTerritories usan claves primarias compuestas
3. **Auto-generación**: Los IDs se generan automáticamente por SQL Server (IDENTITY)
4. **Valores Nulos**: Muchos campos son opcionales (NULL)
5. **Jerarquía Empleados**: Employees.ReportsTo permite jerarquía de supervisión
6. **Fotos**: El campo Photo en Employees almacena datos binarios de imagen
