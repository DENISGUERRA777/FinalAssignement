using NorthwindTradersApp.Domain.Contracts;
namespace NorthwindTradersApp.Infrastructure;

/// <summary>
/// Repository for the Orders entity. This is responsible for handling all data access related to orders.
///  It implements the IOrdersRepository interface, which defines the contract for the Orders repository.
/// </summary>
public sealed class OrdersRepository : IOrdersRepository
{
    private readonly NorthwindDbContext _dbContext;

    public OrdersRepository(NorthwindDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        return await _dbContext.Orders
            .Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                EmployeeId = o.EmployeeId,
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate,
                ShippedDate = o.ShippedDate,
                ShipVia = o.ShipVia,
                Freight = o.Freight,
                ShipName = o.ShipName,
                ShipAddress = o.ShipAddress,
                ShipCity = o.ShipCity,
                ShipRegion = o.ShipRegion,
                ShipPostalCode = o.ShipPostalCode,
                ShipCountry = o.ShipCountry
            })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<OrderDto>> SearchOrderAsync(int? OrderId, string? CustomerName, CancellationToken ct = default)
    {
        
        var query = from o in _dbContext.Orders
                    join c in _dbContext.Customers on o.CustomerId equals c.CustomerId into oc
                    from c in oc.DefaultIfEmpty()
                    select new { o, c };

        if (OrderId.HasValue)
        {
            query = query.Where(o => o.OrderId == OrderId.Value);
        }
        if (!string.IsNullOrEmpty(CustomerName))
        {
            query =  query.Where(o => o.c != null && o.c.CustomerName.Contains(CustomerName));
        }

        return await query.Select(o => new OrderDto
        {
            OrderId = o.o.OrderId,
            CustomerId = o.o.CustomerId,
            EmployeeId = o.o.EmployeeId,
            OrderDate = o.o.OrderDate,
            RequiredDate = o.o.RequiredDate,
            ShippedDate = o.o.ShippedDate,
            ShipVia = o.o.ShipVia,
            Freight = o.o.Freight,
            ShipName = o.o.ShipName,
            ShipAddress = o.o.ShipAddress,
            ShipCity = o.o.ShipCity,
            ShipRegion = o.o.ShipRegion,
            ShipPostalCode = o.o.ShipPostalCode,
            ShipCountry = o.o.ShipCountry
        }.ToListAsync(ct));
    }

    public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto, CancellationToken ct = default)
    {
        var order = new Order
        {
            CustomerId = orderDto.CustomerId,
            EmployeeId = orderDto.EmployeeId,
            OrderDate = orderDto.OrderDate,
            RequiredDate = orderDto.RequiredDate,
            ShippedDate = orderDto.ShippedDate,
            ShipVia = orderDto.ShipVia,
            Freight = orderDto.Freight,
            ShipName = orderDto.ShipName,
            ShipAddress = orderDto.ShipAddress,
            ShipCity = orderDto.ShipCity,
            ShipRegion = orderDto.ShipRegion,
            ShipPostalCode = orderDto.ShipPostalCode,
            ShipCountry = orderDto.ShipCountry
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(ct);

        // Map the created order back to OrderDto
        return new OrderDto
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            EmployeeId = order.EmployeeId,
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            ShipVia = order.ShipVia,
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShipAddress = order.ShipAddress,
            ShipCity = order.ShipCity,
            ShipRegion = order.ShipRegion,
            ShipPostalCode = order.ShipPostalCode,
            ShipCountry = order.ShipCountry
        };
    }

    public async Task<OrderDto?> UpdateOrderAsync(int orderId, OrderDto orderDto, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Orders
            .Where(o => o.OrderId == orderId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(o => o.CustomerId, orderDto.CustomerId)
                .SetProperty(o => o.EmployeeId, orderDto.EmployeeId)
                .SetProperty(o => o.OrderDate, orderDto.OrderDate)
                .SetProperty(o => o.RequiredDate, orderDto.RequiredDate)
                .SetProperty(o => o.ShippedDate, orderDto.ShippedDate)
                .SetProperty(o => o.ShipVia, orderDto.ShipVia)
                .SetProperty(o => o.Freight, orderDto.Freight)
                .SetProperty(o => o.ShipName, orderDto.ShipName)
                .SetProperty(o => o.ShipAddress, orderDto.ShipAddress)
                .SetProperty(o => o.ShipCity, orderDto.ShipCity)
                .SetProperty(o => o.ShipRegion, orderDto.ShipRegion)
                .SetProperty(o => o.ShipPostalCode, orderDto.ShipPostalCode)
                .SetProperty(o => o.ShipCountry, orderDto.ShipCountry), ct);

        if (affectedRows == 0)
        {throw new KeyNotFoundException($"Order with ID {orderId} not found.");}
        // Map the updated order back to OrderDto
        return new OrderDto
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            EmployeeId = order.EmployeeId,
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            ShipVia = order.ShipVia,
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShipAddress = order.ShipAddress,
            ShipCity = order.ShipCity,
            ShipRegion = order.ShipRegion,
            ShipPostalCode = order.ShipPostalCode,
            ShipCountry = order.ShipCountry
        };
    }

    public async Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct = default)
    {
        var affected = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.OrderId == orderId, ct);
        if (affected == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        _dbContext.Orders.Remove(affected);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}