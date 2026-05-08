namespace NorthwindTradersApp.Domain.Contracts;

/// <summary>
/// Interface for the Orders service. This defines the contract for the Orders
///  service, which is responsible for handling all business logic related to orders.
/// </summary>
public interface IOrdersService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken ct= default);
    Task<IEnumerable<OrderDto>> SearchOrderAsync(int? OrderId, string? CustomerName, CancellationToken ct = default);
    Task<OrderDto> CreateOrderAsync(OrderDto orderDto, CancellationToken ct = default);
    Task<OrderDto> UpdateOrderAsync(int orderId, OrderDto orderDto, CancellationToken ct = default);
    Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct = default);
}

/// <summary>
/// Interface for the Products service. This defines the contract for the Products
/// service, which is responsible for handling all business logic related to products.
/// </summary>
public interface IProductsService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken ct = default);
    Task<IEnumerable<ProductDto>> SearchProductAsync(string? name, CancellationToken ct = default);
    Task<ProductDto> CreateProductAsync(ProductDto productDto, CancellationToken ct = default);
    Task<ProductDto> UpdateProductAsync(int productId, ProductDto productDto, CancellationToken ct = default);
    Task<bool> DeleteProductAsync(int productId, CancellationToken ct = default);
}

/// <summary>
/// Interface for the Customers service. This defines the contract for the Customers
///  service, which is responsible for handling all business logic related to customers.
/// </summary>
public interface ICustomersService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken ct = default);
    Task<IEnumerable<CustomerDto>> SearchCustomerAsync(string? name, CancellationToken ct = default);
    Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, CancellationToken ct = default);
    Task<CustomerDto> UpdateCustomerAsync(string customerId, CustomerDto customerDto, CancellationToken ct = default);
    Task<bool> DeleteCustomerAsync(string customerId, CancellationToken ct = default);
}



/// <summary>
/// Interface for the Orders repository. This defines the contract for the Orders repository,
///  which is responsible for handling all data access related to orders.
/// </summary>
public interface IOrdersRepository
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default);
    Task<IEnumerable<OrderDto>> SearchOrderAsync(int? OrderId, string? CustomerName, CancellationToken ct = default);
    Task<OrderDto> CreateOrderAsync(OrderDto orderDto, CancellationToken ct = default);
    Task<OrderDto> UpdateOrderAsync(int orderId, OrderDto orderDto, CancellationToken ct = default);
    Task<bool> DeleteOrderAsync(int orderId, CancellationToken ct = default);
}

/// <summary>
/// Interface for the Products repository. This defines the contract for the Products repository,
///  which is responsible for handling all data access related to products.
/// </summary>
public interface IProductsRepository
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken ct = default);
    Task<IEnumerable<ProductDto>> SearchProductAsync(string? name, CancellationToken ct = default);
    Task<ProductDto> CreateProductAsync(ProductDto productDto, CancellationToken ct = default);
    Task<ProductDto> UpdateProductAsync(int productId, ProductDto productDto, CancellationToken ct = default);
    Task<bool> DeleteProductAsync(int productId, CancellationToken ct = default);
}

/// <summary>
/// Interface for the Customers repository. This defines the contract for the Customers repository,
/// which is responsible for handling all data access related to customers.
/// </summary>
public interface ICustomersRepository
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken ct = default);
    Task<IEnumerable<CustomerDto>> SearchCustomerAsync(string? name, CancellationToken ct = default);
    Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto, CancellationToken ct = default);
    Task<CustomerDto> UpdateCustomerAsync(string customerId, CustomerDto customerDto, CancellationToken ct = default);
    Task<bool> DeleteCustomerAsync(string customerId, CancellationToken ct = default);
}
