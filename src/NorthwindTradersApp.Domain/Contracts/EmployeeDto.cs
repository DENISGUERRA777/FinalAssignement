namespace NorthwindTradersApp.Domain.Contracts;
/// <summary>
/// DTO for the Employee entity.
/// This is used to transfer data between
/// layers of the application without exposing the internal structure of the entity.
/// </summary>
public sealed class EmployeeDto
{
    public int EmployeeId { get; set; }
    public string LastName { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? Title { get; set; }
    public string? TitleOfCourtesy { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? HireDate { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? HomePhone { get; set; }
    public string? Extension { get; set; }
}