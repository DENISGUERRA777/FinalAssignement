namespace NorthwindTradersApp.Domain.NorthwindDbEntities;
/// <summary>
/// Maps to the dbo.Shippers table in the Northwind database.
/// Schema: dbo , Primary Key: ShipperId
/// </summary>
public sealed class Shipper
{
    public int ShipperId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string Phone { get; set; } = null!;
}
