namespace NorthwindTradersApp.Domain.Contracts;
/// <summary>
/// DTO for the Product entity. This is used to transfer data between layers of the application without exposing the internal structure of the entity.
/// Rquired joint; category and supplier are required to create a product,
///  but not required to update a product. This is because the product may
///  already have a category and supplier assigned to it,
///  and we don't want to force the client to provide that information
///  when updating the product.
/// </summary>
public sealed class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int? SupplierId { get; set; }
    public int? CategoryId { get; set; }
    public string? QuantityPerUnit { get; set; }
    public decimal? UnitPrice { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public short? ReorderLevel { get; set; }
    public bool Discontinued { get; set; }

    //Category name  from dbo.Categories
    public string? CategoryName { get; set; }
    //Supplier name from dbo.Suppliers
    public string? SupplierName { get; set; }
}

/// <summary>
/// DTO for the Order entity. This is used to transfer data between layers of the application without
/// </summary>
public sealed class OrderDto
{
    public int OrderId { get; set; }
    public int? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public int? ShipVia { get; set; }
    public decimal? Freight { get; set; }
    public string? ShipName { get; set; }
    public string? ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipRegion { get; set; }
    public string? ShipPostalCode { get; set; }
    public string? ShipCountry { get; set; }

    //Customer name from dbo.Customers
    public string? CustomerName { get; set; }
    //Employee name from dbo.Employees
    public string? EmployeeName { get; set; }
    //Shipper name from dbo.Shippers
    public string? ShipperName { get; set; }
}