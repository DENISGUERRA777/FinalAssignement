namespace NorthwindTradersApp.Domain.Contracts;

/// <summary>
/// DTO for the Product entity.
/// This is used to transfer data between
///  layers of the application without exposing the internal structure of the entity.
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

    //Category name from the Categories table
    public string? CategoryName { get; set; }

}