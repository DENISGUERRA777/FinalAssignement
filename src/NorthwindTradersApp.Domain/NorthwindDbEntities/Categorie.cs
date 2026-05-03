namespace NorthwindTradersApp.Domain.NorthwindDbEntities;

/// <summary>
/// Maps to the dbo.Categories table in the Northwind database.
/// Schema: dbo , Primary Key: CategoryId
/// </summary>
public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public byte[]? Picture { get; set; }
}