namespace NorthwindTradersApp.Domain.NorthwindDbEntities;
/// <summary>
/// Maps to the dbo.Region table in the Northwind database.
/// Schema: dbo , Primary Key: RegionId
/// </summary>
public sealed class Region
{
    public int RegionId { get; set; }
    public string RegionDescription { get; set; } = null!;
}
