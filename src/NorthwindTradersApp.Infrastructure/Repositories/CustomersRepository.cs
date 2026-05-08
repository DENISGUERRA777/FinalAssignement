using Microsoft.EntityFrameworkCore;
using NorthwindTradersApp.Domain.Contracts;
using NorthwindTradersApp.Domain.NorthwindDbEntities;
using NorthwindTradersApp.Infrastructure.Persistence;

namespace NorthwindTradersApp.Infrastructure.Repositories;

/// <summary>
/// Repository for the Customers entity.
///  This is responsible for handling all data access related to customers.
/// </summary>
public sealed class CustomersRepository : ICustomersRepository
{
    private readonly NorthwindDbContext _dbContext;

    public CustomersRepository(NorthwindDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken ct = default)
    {
        return await _dbContext.Customers
            .Select(c => new CustomerDto
            {
                CustomerId = c.CustomerId,
                CompanyName = c.CompanyName,
                ContactName = c.ContactName,
                ContactTitle = c.ContactTitle,
                Address = c.Address,
                City = c.City,
                Region = c.Region,
                PostalCode = c.PostalCode,
                Country = c.Country,
                Phone = c.Phone,
                Fax = c.Fax
            }).ToListAsync(ct);
    }

    public async Task<IEnumerable<CustomerDto>> SearchCustomerAsync(string? name, CancellationToken ct = default)
    {
        var query = _dbContext.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.CompanyName.Contains(name) || (c.ContactName != null && c.ContactName.Contains(name)));
        }
        return await query.Select(c => new CustomerDto
        {
            CustomerId = c.CustomerId,
            CompanyName = c.CompanyName,
            ContactName = c.ContactName,
            ContactTitle = c.ContactTitle,
            Address = c.Address,
            City = c.City,
            Region = c.Region,
            PostalCode = c.PostalCode,
            Country = c.Country,
            Phone = c.Phone,
            Fax = c.Fax
        }).ToListAsync(ct);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, CancellationToken ct = default)
    {
        var customer = new Customer
        {
            CustomerId = customerDto.CustomerId,
            CompanyName = customerDto.CompanyName,
            ContactName = customerDto.ContactName,
            ContactTitle = customerDto.ContactTitle,
            Address = customerDto.Address,
            City = customerDto.City,
            Region = customerDto.Region,
            PostalCode = customerDto.PostalCode,
            Country = customerDto.Country,
            Phone = customerDto.Phone,
            Fax = customerDto.Fax
        };
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(ct);
        return customerDto;
    }

    public async Task<CustomerDto> UpdateCustomerAsync(string customerId, CustomerDto customerDto, CancellationToken ct = default)
    {
        var affectedCustomers = await _dbContext.Customers.
        Where(c => c.CustomerId == customerId)
        .ExecuteUpdateAsync(s => s
            .SetProperty(c => c.CompanyName, customerDto.CompanyName)
            .SetProperty(c => c.ContactName, customerDto.ContactName)
            .SetProperty(c => c.ContactTitle, customerDto.ContactTitle)
            .SetProperty(c => c.Address, customerDto.Address)
            .SetProperty(c => c.City, customerDto.City)
            .SetProperty(c => c.Region, customerDto.Region)
            .SetProperty(c => c.PostalCode, customerDto.PostalCode)
            .SetProperty(c => c.Country, customerDto.Country)
            .SetProperty(c => c.Phone, customerDto.Phone)
            .SetProperty(c => c.Fax, customerDto.Fax), ct);

        if (affectedCustomers == 0)
        {
            throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
        }

        customerDto.CustomerId = customerId;
        return customerDto;
    }

    public async Task<bool> DeleteCustomerAsync(string customerId, CancellationToken ct = default)
    {
        var entity = await _dbContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId, ct);
        if (entity == null)        {
            throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
        }
        _dbContext.Customers.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}
