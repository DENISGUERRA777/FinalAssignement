using NorthwindTradersApp.Domain.Contracts;

namespace NorthwindTradersApp.Application.Services;

/// <summary>
/// Implementation of the IProductsService interface.
/// </summary>
public sealed class ProductService : IProductsService
{
    private readonly IProductsRepository _productsRepository;

    public ProductService(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository;
    }

    public Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken ct = default)
    {
        return _productsRepository.GetAllProductsAsync(ct);
    }

    public Task<IEnumerable<ProductDto>> SearchProductAsync(string? name, CancellationToken ct = default)
    {
        return _productsRepository.SearchProductAsync(name, ct);
    }

    public Task<ProductDto> CreateProductAsync(ProductDto productDto, CancellationToken ct = default)
    {
        return _productsRepository.CreateProductAsync(productDto, ct);
    }

    public Task<ProductDto> UpdateProductAsync(int productId, ProductDto productDto, CancellationToken ct = default)
    {
        return _productsRepository.UpdateProductAsync(productId, productDto, ct);
    }

    public Task<bool> DeleteProductAsync(int productId, CancellationToken ct = default)
    {
        return _productsRepository.DeleteProductAsync(productId, ct);
    }
}