using NorthwindTradersApp.Domain.Contracts;

namespace NorthwindTradersApp.Application.Services;

/// <summary>
/// Implementation of the ICustomersService interface.
/// </summary>
public sealed class CustomersService : ICustomersService
{
    private readonly ICustomersRepository _customersRepository;

    public CustomersService(ICustomersRepository customersRepository)
    {
        _customersRepository = customersRepository;
    }

    public Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken ct = default)
    {
        return _customersRepository.GetAllCustomersAsync(ct);
    }

    public Task<IEnumerable<CustomerDto>> SearchCustomerAsync(string? customerName, CancellationToken ct = default)
    {
        return _customersRepository.SearchCustomerAsync(customerName, ct);
    }

    public Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, CancellationToken ct = default)
    {
        return _customersRepository.CreateCustomerAsync(customerDto, ct);
    }

    public Task<CustomerDto> UpdateCustomerAsync(string customerId, CustomerDto customerDto, CancellationToken ct = default)
    {
        return _customersRepository.UpdateCustomerAsync(customerId, customerDto, ct);
    }

    public Task<bool> DeleteCustomerAsync(string customerId, CancellationToken ct = default)
    {
        return _customersRepository.DeleteCustomerAsync(customerId, ct);
    }
}