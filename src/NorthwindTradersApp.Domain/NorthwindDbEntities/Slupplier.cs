namespace NorthwindTradersApp.Domain.NorthwindDbEntities;
/// <summary>
/// Maps to the dbo.Suppliers table in the Northwind database.
/// Schema: dbo , Primary Key: SupplierId
/// </summary>
public sealed class Supplier
{
    public int SupplierId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string ContactName { get; set; } = null!;
    public string ContactTitle { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Region { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Fax { get; set; } = null!;
    public string HomePage { get; set; } = null!;
}
