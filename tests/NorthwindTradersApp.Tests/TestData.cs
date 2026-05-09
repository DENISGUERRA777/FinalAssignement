using NorthwindTradersApp.Domain.Contracts;

namespace NorthwindTradersApp.Tests;

internal static class TestData
{
    public static CustomerDto CreateCustomer(string customerId = "ALFKI")
    {
        return new CustomerDto
        {
            CustomerId = customerId,
            CompanyName = "Alfreds Futterkiste",
            ContactName = "Maria Anders",
            ContactTitle = "Sales Representative",
            City = "Berlin",
            Country = "Germany"
        };
    }

    public static ProductDto CreateProduct(int productId = 1)
    {
        return new ProductDto
        {
            ProductId = productId,
            ProductName = "Chai",
            CategoryName = "Beverages",
            UnitPrice = 18.0m,
            UnitsInStock = 39,
            Discontinued = false
        };
    }

    public static OrderDto CreateOrder(int orderId = 10248)
    {
        return new OrderDto
        {
            OrderId = orderId,
            CustomerId = "ALFKI",
            Freight = 32.38m,
            ShipCountry = "Germany",
            OrderDetails = new List<OrderDetailDto>
            {
                new()
                {
                    OrderId = orderId,
                    ProductId = 11,
                    UnitPrice = 14.0m,
                    Quantity = 12,
                    Discount = 0
                }
            }
        };
    }
}
