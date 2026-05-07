using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using NorthwindTradersApp.Domain.Contracts;
using NorthwindTradersApp.Domain.NorthwindDbEntities;
using NorthwindTradersApp.Infrastructure.Persistence;

namespace NorthwindTradersApp.Infrastructure.Repositories;

/// <summary>
/// Repository for the Products entity. This is responsible for handling all data access related to products.
///  It implements the IProductsRepository interface, which defines the contract for the Products repository.
/// </summary>
public sealed class ProductsRepository : IProductsRepository
{
    private readonly NorthwindDbContext _dbContext;

    public ProductsRepository(NorthwindDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken ct = default)
    {
        return await(from p in _dbContext.Products
        join c in _dbContext.Categories 
        on p.CategoryId equals c.CategoryId into pc
        from c in pc.DefaultIfEmpty()
        select new ProductDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            SupplierId = p.SupplierId,
            CategoryId = p.CategoryId,
            QuantityPerUnit = p.QuantityPerUnit,
            UnitPrice = p.UnitPrice,
            UnitsInStock = p.UnitsInStock,
            UnitsOnOrder = p.UnitsOnOrder,
            ReorderLevel = p.ReorderLevel,
            Discontinued = p.Discontinued,
            CategoryName = c != null ? c.CategoryName : null
        }).ToListAsync(ct);
        
    }

    public async Task<IEnumerable<ProductDto>> SearchProductAsync(string? productName, CancellationToken ct = default)
    {
        var query = from p in _dbContext.Products
        join c in _dbContext.Categories 
        on p.CategoryId equals c.CategoryId into pc
        from c in pc.DefaultIfEmpty()
        select new 
        {p, c};

        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(x => x.p.ProductName.Contains(productName));
        }

        return await query.Select(x => new ProductDto
        {
            ProductId = x.p.ProductId,
            ProductName = x.p.ProductName,
            SupplierId = x.p.SupplierId,
            CategoryId = x.p.CategoryId,
            QuantityPerUnit = x.p.QuantityPerUnit,
            UnitPrice = x.p.UnitPrice,
            UnitsInStock = x.p.UnitsInStock,
            UnitsOnOrder = x.p.UnitsOnOrder,
            ReorderLevel = x.p.ReorderLevel,
            Discontinued = x.p.Discontinued,
            CategoryName = x.c != null ? x.c.CategoryName : null
        }).ToListAsync(ct);
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto, CancellationToken ct = default)
    {
        var product = new Product
        {
            ProductName = productDto.ProductName,
            SupplierId = productDto.SupplierId,
            CategoryId = productDto.CategoryId,
            QuantityPerUnit = productDto.QuantityPerUnit,
            UnitPrice = productDto.UnitPrice,
            UnitsInStock = productDto.UnitsInStock,
            UnitsOnOrder = productDto.UnitsOnOrder,
            ReorderLevel = productDto.ReorderLevel,
            Discontinued = productDto.Discontinued
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(ct);

        productDto.ProductId = product.ProductId;
        return productDto;
    }

    public async Task<ProductDto> UpdateProductAsync(int productId, ProductDto productDto, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Products
            .Where(p => p.ProductId == productId)
            .ExecuteUpdateAsync(p => p
                .SetProperty(p => p.ProductName, productDto.ProductName)
                .SetProperty(p => p.SupplierId, productDto.SupplierId)
                .SetProperty(p => p.CategoryId, productDto.CategoryId)
                .SetProperty(p => p.QuantityPerUnit, productDto.QuantityPerUnit)
                .SetProperty(p => p.UnitPrice, productDto.UnitPrice)
                .SetProperty(p => p.UnitsInStock, productDto.UnitsInStock)
                .SetProperty(p => p.UnitsOnOrder, productDto.UnitsOnOrder)
                .SetProperty(p => p.ReorderLevel, productDto.ReorderLevel)
                .SetProperty(p => p.Discontinued, productDto.Discontinued), ct);

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException($"Product with ID {productId} not found.");
        }

        productDto.ProductId = productId;
        return productDto;
    }

    public async Task<bool> DeleteProductAsync(int productId, CancellationToken ct = default)
    {
        var entity = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId, ct);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Product with ID {productId} was not found.");
        }

        _dbContext.Products.Remove(entity);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}