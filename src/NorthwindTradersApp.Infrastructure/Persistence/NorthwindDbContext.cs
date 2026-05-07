using System.Dynamic;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using NorthwindTradersApp.Domain.NorthwindDbEntities;
namespace NorthwindTradersApp.Infrastructure.Persistence;

/// <summary>
/// DbContext for the Northwind database.
///  This is used to interact with the database using Entity Framework Core.   
/// </summary>
public sealed class NorthwindDbContext : DbContext
{
    public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Shipper> Shippers => Set<Shipper>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Territory> Territories => Set<Territory>();
    public DbSet<EmployeeTerritory> EmployeeTerritories => Set<EmployeeTerritory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Product>(EntityHandle =>{
            EntityHandle.ToTable("Products", "dbo");
            EntityHandle.HasKey(p => p.ProductId);
        });

        modelBuilder.Entity<Order>(EntityHandle =>{
            EntityHandle.ToTable("Orders", "dbo");
            EntityHandle.HasKey(o => o.OrderId);
        });

        modelBuilder.Entity<Category>(EntityHandle =>{
            EntityHandle.ToTable("Categories", "dbo");
            EntityHandle.HasKey(c => c.CategoryId);
        });

        modelBuilder.Entity<Supplier>(EntityHandle =>{
            EntityHandle.ToTable("Suppliers", "dbo");
            EntityHandle.HasKey(s => s.SupplierId);
        });

        modelBuilder.Entity<Customer>(EntityHandle =>{
            EntityHandle.ToTable("Customers", "dbo");
            EntityHandle.HasKey(c => c.CustomerId);
        });

        modelBuilder.Entity<Employee>(EntityHandle =>{
            EntityHandle.ToTable("Employees", "dbo");
            EntityHandle.HasKey(e => e.EmployeeId);
        });

        modelBuilder.Entity<Shipper>(EntityHandle =>{
            EntityHandle.ToTable("Shippers", "dbo");
            EntityHandle.HasKey(s => s.ShipperId);
        });

        modelBuilder.Entity<OrderDetail>(EntityHandle =>{
            EntityHandle.ToTable("Order Details", "dbo");
            EntityHandle.HasKey(od => new { od.OrderId, od.ProductId });
        });

        modelBuilder.Entity<Region>(EntityHandle =>{
            EntityHandle.ToTable("Region", "dbo");
            EntityHandle.HasKey(r => r.RegionId);
        });

        modelBuilder.Entity<Territory>(EntityHandle =>{
            EntityHandle.ToTable("Territories", "dbo");
            EntityHandle.HasKey(t => t.TerritoryId);
        });

        modelBuilder.Entity<EmployeeTerritory>(EntityHandle =>{
            EntityHandle.ToTable("EmployeeTerritories", "dbo");
            EntityHandle.HasKey(et => new { et.EmployeeId, et.TerritoryId });
        });
            
    }
}
