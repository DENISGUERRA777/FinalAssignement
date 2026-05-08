using Microsoft.EntityFrameworkCore;
using NorthwindTradersApp.Domain.Contracts;
using NorthwindTradersApp.Domain.NorthwindDbEntities;
using NorthwindTradersApp.Infrastructure.Persistence;

namespace NorthwindTradersApp.Infrastructure.Repositories;

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
        .Include(o => o.OrderDetails) 
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
                    ShipCountry = o.ShipCountry,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderId = od.OrderId,
                        ProductId = od.ProductId,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        Discount = od.Discount
                    }).ToList()
                }).ToListAsync(ct);
        
    }

    public async Task<IEnumerable<OrderDto>> SearchOrderAsync(int? OrderId, string? CustomerName, CancellationToken ct = default)
    {
        
        var query = from o in _dbContext.Orders.Include(o => o.OrderDetails)
                    join c in _dbContext.Customers on o.CustomerId equals c.CustomerId into oc
                    from c in oc.DefaultIfEmpty()
                    select new { o, c };

        if (OrderId.HasValue)
        {
            query = query.Where(x => x.o.OrderId == OrderId.Value);
        }
        if (!string.IsNullOrEmpty(CustomerName))
        {
            query =  query.Where(x => x.c != null && x.c.CompanyName.Contains(CustomerName));
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
            ShipCountry = o.o.ShipCountry,
            OrderDetails = o.o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderId = od.OrderId,
                        ProductId = od.ProductId,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        Discount = od.Discount
                    }).ToList()
        }).ToListAsync(ct);
    }

    public async Task<OrderDto> CreateOrderAsync(OrderDto orderDto, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        
        var order = new Domain.NorthwindDbEntities.Order
        {
            CustomerId = orderDto.CustomerId,
            EmployeeId = orderDto.EmployeeId,
            OrderDate = now,
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

        var orderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
        {
            OrderId = order.OrderId,
            ProductId = od.ProductId,
            UnitPrice = od.UnitPrice,
            Quantity = od.Quantity,
            Discount = od.Discount
        }).ToList();
        
        _dbContext.OrderDetails.AddRange(orderDetails);
        await _dbContext.SaveChangesAsync(ct);

        orderDto.OrderId = order.OrderId;
        foreach (var od in orderDto.OrderDetails)
        {
            od.OrderId = order.OrderId;
        }
        return orderDto;
    }

    public async Task<OrderDto> UpdateOrderAsync(int orderId, OrderDto orderDto, CancellationToken ct = default)
    {
        
        var existingOrder = await _dbContext.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderId == orderId, ct);

        if (existingOrder == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");
        }

        existingOrder.OrderDate = orderDto.OrderDate;
        existingOrder.RequiredDate = orderDto.RequiredDate;
        existingOrder.ShippedDate = orderDto.ShippedDate;
        existingOrder.ShipVia = orderDto.ShipVia;
        existingOrder.Freight = orderDto.Freight;
        existingOrder.ShipName = orderDto.ShipName;
        existingOrder.ShipAddress = orderDto.ShipAddress;
        existingOrder.ShipCity = orderDto.ShipCity;
        existingOrder.ShipRegion = orderDto.ShipRegion;
        existingOrder.ShipPostalCode = orderDto.ShipPostalCode;
        existingOrder.ShipCountry = orderDto.ShipCountry;

        var newDetailsEntities = orderDto.OrderDetails.Select(d => new OrderDetail
        {
            OrderId = orderId, 
            ProductId = d.ProductId,
            UnitPrice = d.UnitPrice,
            Quantity = d.Quantity,
            Discount = d.Discount
        }).ToList();

        existingOrder.OrderDetails = newDetailsEntities;

        await _dbContext.SaveChangesAsync(ct);

        orderDto.OrderId = orderId;
        return orderDto;
    }

    public async Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct = default)
    {
        var affected = await _dbContext.Orders.Include(o => o.OrderDetails)
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