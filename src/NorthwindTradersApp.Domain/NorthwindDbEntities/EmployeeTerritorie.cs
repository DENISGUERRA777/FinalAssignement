namespace NorthwindTradersApp.Domain.NorthwindDbEntities;
/// <summary>
/// Maps to the dbo.EmployeeTerritories table in the Northwind database.
/// Schema: dbo , Primary Key: EmployeeId, TerritoryId
/// </summary>
public class EmployeeTerritory
{
    public int EmployeeId { get; set; }
    public string TerritoryId { get; set; } = null!;
}