namespace P2.LoadGenerator.Execution;

/// <summary>
/// Generates synthetic <see cref="CustomerRequest"/> instances that mimic distinct shoppers.
/// </summary>
public interface ICustomerRequestFactory
{
    CustomerRequest Create();
}

/// <summary>
/// Default factory: picks a random synthetic customer and a random product from a small
/// fixed catalogue. Swap in a factory backed by P1's real dataset once it exists.
/// </summary>
public sealed class RandomCustomerRequestFactory : ICustomerRequestFactory
{
    private readonly Random _random;
    private readonly int _customerPoolSize;
    private readonly string[] _productCatalogue;

    public RandomCustomerRequestFactory(
        int customerPoolSize = 500,
        string[]? productCatalogue = null,
        int? seed = null)
    {
        _customerPoolSize = customerPoolSize;
        _productCatalogue = productCatalogue ?? new[] { "SKU-1001", "SKU-1002", "SKU-1003", "SKU-1004", "SKU-1005" };
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public CustomerRequest Create()
    {
        var customerId = $"cust-{_random.Next(1, _customerPoolSize + 1)}";
        var productId = _productCatalogue[_random.Next(_productCatalogue.Length)];
        var quantity = _random.Next(1, 4).ToString();

        return new CustomerRequest
        {
            RequestId = Guid.NewGuid(),
            CustomerId = customerId,
            Parameters = new Dictionary<string, string>
            {
                ["productId"] = productId,
                ["qty"] = quantity
            }
        };
    }
}
