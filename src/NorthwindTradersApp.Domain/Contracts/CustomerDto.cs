namespace NorthwindTradersApp.Domain.Contracts;
/// <summary>
/// DTO for the Customer entity. 
/// This is used to transfer data between
///  layers of the application without exposing the internal structure of the entity.   
/// </summary>
public sealed class CustomerDto
{
    public string CustomerId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}