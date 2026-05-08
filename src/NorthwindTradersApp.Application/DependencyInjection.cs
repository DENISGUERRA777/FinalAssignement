using Microsoft.Extensions.DependencyInjection;
using NorthwindTradersApp.Domain.Contracts;
using NorthwindTradersApp.Application.Services;

namespace NorthwindTradersApp.Application;

/// <summary>
/// Registers Application layer services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddScoped<IProductsService, ProductService>();
        services.AddScoped<ICustomersService, CustomersService>();
        return services;
    }
}
