using NorthwindTradersApp.Domain.Contracts;

namespace NorthwindTradersApp.Application.Services;

/// <summary>
/// Implementation of the IOrdersService interface.
/// </summary>
public sealed class OrdersService : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository;

    public OrdersService(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        return _ordersRepository.GetAllOrdersAsync(ct);
    }

    public Task<IEnumerable<OrderDto>> SearchOrderAsync(int? OrderId, string? CustomerName, CancellationToken ct = default)
    {
        return _ordersRepository.SearchOrderAsync(OrderId, CustomerName, ct);
    }

    public Task<OrderDto> CreateOrderAsync(OrderDto orderDto, CancellationToken ct = default)
    {
        return _ordersRepository.CreateOrderAsync(orderDto, ct);
    }

    public Task<OrderDto> UpdateOrderAsync(int orderId, OrderDto orderDto, CancellationToken ct = default)
    {
        return _ordersRepository.UpdateOrderAsync(orderId, orderDto, ct);
    }

    public Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct = default)
    {
        return _ordersRepository.DeleteOrderAsync(orderId, ct);
    }
}