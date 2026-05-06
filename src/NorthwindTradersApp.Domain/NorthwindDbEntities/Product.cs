namespace NorthwindTradersApp.Domain.NorthwindDbEntities;
/// <summary>
/// Maps to the dbo.Products table in the Northwind database.
/// Schema: dbo , Primary Key: ProductId, Foreign Keys: SupplierId, CategoryId
///  references dbo.Suppliers, dbo.Categories respectively.
/// </summary>
public sealed class Product
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
}
