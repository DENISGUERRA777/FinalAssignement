namespace NorthwindTradersApp.Domain.NorthwindDbEntities;
/// <summary>
/// Maps to the dbo.Territories table in the Northwind database.
/// Schema: dbo , Primary Key: TerritoryId, Foreign Key: RegionId references dbo.Regions(RegionId)
/// </summary>
public sealed class Territory
{
    public string TerritoryId { get; set; } = null!;
    public string TerritoryDescription { get; set; } = null!;
    public int RegionId { get; set; }
}
