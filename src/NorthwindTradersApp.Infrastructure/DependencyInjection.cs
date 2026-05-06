using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindTradersApp.Domain.Contracts;
namespace NorthwindTradersApp.Infrastructure;

/// <summary>
/// This class is responsible for registering all the services and repositories
///  in the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the DbContext with the connection string from appsettings.json
        services.AddDbContext<NorthwindDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NorthwindDatabase")));

        // Register repositories and services
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddScoped<IProductsService, ProductsService>();

        return services;
    }
}