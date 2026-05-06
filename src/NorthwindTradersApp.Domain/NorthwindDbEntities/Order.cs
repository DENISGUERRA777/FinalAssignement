namespace NorthwindTradersApp.Domain.NorthwindDbEntities;

/// <summary>
/// Maps to the dbo.Orders table in the Northwind database.
/// Schema: dbo , Primary Key: OrderId, Foreign Keys: CustomerId, EmployeeId, ShipVia
///  references dbo.Customers, dbo.Employees, dbo.Shippers respectively.
/// </summary>
public sealed class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime RequiredDate { get; set; }
    public DateTime? ShippedDate { get; set; }
    public int ShipVia { get; set; }
    public decimal Freight { get; set; }
    public string ShipName { get; set; } = null!;
    public string ShipAddress { get; set; } = null!;
    public string ShipCity { get; set; } = null!;
    public string ShipRegion { get; set; } = null!;
    public string ShipPostalCode { get; set; } = null!;
    public string ShipCountry { get; set; } = null!;
}
