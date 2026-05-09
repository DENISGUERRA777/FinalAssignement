using Moq;
using NorthwindTradersApp.Application.Services;
using NorthwindTradersApp.Domain.Contracts;
using Xunit;

namespace NorthwindTradersApp.Tests;

public sealed class CustomersServiceTests
{
    [Fact]
    public async Task GetAllCustomersAsync_DelegatesToRepository()
    {
        var repository = new Mock<ICustomersRepository>();
        var expected = new[] { TestData.CreateCustomer() };
        repository.Setup(r => r.GetAllCustomersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var service = new CustomersService(repository.Object);

        var result = await service.GetAllCustomersAsync();

        Assert.Same(expected, result);
        repository.Verify(r => r.GetAllCustomersAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchCustomerAsync_DelegatesToRepository()
    {
        var repository = new Mock<ICustomersRepository>();
        var expected = new[] { TestData.CreateCustomer("BERGS") };
        repository.Setup(r => r.SearchCustomerAsync("berg", It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var service = new CustomersService(repository.Object);

        var result = await service.SearchCustomerAsync("berg");

        Assert.Same(expected, result);
        repository.Verify(r => r.SearchCustomerAsync("berg", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCustomerAsync_DelegatesToRepository()
    {
        var repository = new Mock<ICustomersRepository>();
        var input = TestData.CreateCustomer("NEW01");
        repository.Setup(r => r.CreateCustomerAsync(input, It.IsAny<CancellationToken>())).ReturnsAsync(input);

        var service = new CustomersService(repository.Object);

        var result = await service.CreateCustomerAsync(input);

        Assert.Same(input, result);
        repository.Verify(r => r.CreateCustomerAsync(input, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAsync_DelegatesToRepository()
    {
        var repository = new Mock<ICustomersRepository>();
        var input = TestData.CreateCustomer("OLD01");
        repository.Setup(r => r.UpdateCustomerAsync("OLD01", input, It.IsAny<CancellationToken>())).ReturnsAsync(input);

        var service = new CustomersService(repository.Object);

        var result = await service.UpdateCustomerAsync("OLD01", input);

        Assert.Same(input, result);
        repository.Verify(r => r.UpdateCustomerAsync("OLD01", input, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_DelegatesToRepository()
    {
        var repository = new Mock<ICustomersRepository>();
        repository.Setup(r => r.DeleteCustomerAsync("OLD01", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new CustomersService(repository.Object);

        var result = await service.DeleteCustomerAsync("OLD01");

        Assert.True(result);
        repository.Verify(r => r.DeleteCustomerAsync("OLD01", It.IsAny<CancellationToken>()), Times.Once);
    }
}

public sealed class OrdersServiceTests
{
    [Fact]
    public async Task GetAllOrdersAsync_DelegatesToRepository()
    {
        var repository = new Mock<IOrdersRepository>();
        var expected = new[] { TestData.CreateOrder() };
        repository.Setup(r => r.GetAllOrdersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var service = new OrdersService(repository.Object);

        var result = await service.GetAllOrdersAsync();

        Assert.Same(expected, result);
        repository.Verify(r => r.GetAllOrdersAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchOrderAsync_DelegatesToRepository()
    {
        var repository = new Mock<IOrdersRepository>();
        var expected = new[] { TestData.CreateOrder(2) };
        repository.Setup(r => r.SearchOrderAsync(2, "alf", It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var service = new OrdersService(repository.Object);

        var result = await service.SearchOrderAsync(2, "alf");

        Assert.Same(expected, result);
        repository.Verify(r => r.SearchOrderAsync(2, "alf", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_DelegatesToRepository()
    {
        var repository = new Mock<IOrdersRepository>();
        var input = TestData.CreateOrder(3);
        repository.Setup(r => r.CreateOrderAsync(input, It.IsAny<CancellationToken>())).ReturnsAsync(input);

        var service = new OrdersService(repository.Object);

        var result = await service.CreateOrderAsync(input);

        Assert.Same(input, result);
        repository.Verify(r => r.CreateOrderAsync(input, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderAsync_DelegatesToRepository()
    {
        var repository = new Mock<IOrdersRepository>();
        var input = TestData.CreateOrder(4);
        repository.Setup(r => r.UpdateOrderAsync(4, input, It.IsAny<CancellationToken>())).ReturnsAsync(input);

        var service = new OrdersService(repository.Object);

        var result = await service.UpdateOrderAsync(4, input);

        Assert.Same(input, result);
        repository.Verify(r => r.UpdateOrderAsync(4, input, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOrderAsync_DelegatesToRepository()
    {
        var repository = new Mock<IOrdersRepository>();
        repository.Setup(r => r.DeleteOrderAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new OrdersService(repository.Object);

        var result = await service.DeleteOrderAsync(4);

        Assert.True(result);
        repository.Verify(r => r.DeleteOrderAsync(4, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public sealed class ProductServiceTests
{
    [Fact]
    public async Task GetAllProductsAsync_DelegatesToRepository()
    {
        var repository = new Mock<IProductsRepository>();
        var expected = new[] { TestData.CreateProduct() };
        repository.Setup(r => r.GetAllProductsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var service = new ProductService(repository.Object);

        var result = await service.GetAllProductsAsync();

        Assert.Same(expected, result);
        repository.Verify(r => r.GetAllProductsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchProductAsync_DelegatesToRepository()
    {
        var repository = new Mock<IProductsRepository>();
        var expected = new[] { TestData.CreateProduct(2) };
        repository.Setup(r => r.SearchProductAsync("chai", It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var service = new ProductService(repository.Object);

        var result = await service.SearchProductAsync("chai");

        Assert.Same(expected, result);
        repository.Verify(r => r.SearchProductAsync("chai", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_DelegatesToRepository()
    {
        var repository = new Mock<IProductsRepository>();
        var input = TestData.CreateProduct(3);
        repository.Setup(r => r.CreateProductAsync(input, It.IsAny<CancellationToken>())).ReturnsAsync(input);

        var service = new ProductService(repository.Object);

        var result = await service.CreateProductAsync(input);

        Assert.Same(input, result);
        repository.Verify(r => r.CreateProductAsync(input, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_DelegatesToRepository()
    {
        var repository = new Mock<IProductsRepository>();
        var input = TestData.CreateProduct(4);
        repository.Setup(r => r.UpdateProductAsync(4, input, It.IsAny<CancellationToken>())).ReturnsAsync(input);

        var service = new ProductService(repository.Object);

        var result = await service.UpdateProductAsync(4, input);

        Assert.Same(input, result);
        repository.Verify(r => r.UpdateProductAsync(4, input, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_DelegatesToRepository()
    {
        var repository = new Mock<IProductsRepository>();
        repository.Setup(r => r.DeleteProductAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new ProductService(repository.Object);

        var result = await service.DeleteProductAsync(4);

        Assert.True(result);
        repository.Verify(r => r.DeleteProductAsync(4, It.IsAny<CancellationToken>()), Times.Once);
    }
}
