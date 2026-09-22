using P2.LoadGenerator.Catalog;

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

/// <summary>
/// Same shape as <see cref="RandomCustomerRequestFactory"/>, but picks real product IDs,
/// categories, and price-appropriate quantities from P1's product catalogue (see
/// <see cref="P2.LoadGenerator.Catalog.ProductCatalogLoader"/>) instead of five fake SKUs.
/// This is the "data information from P1" the load generator feeds into requests.
/// </summary>
public sealed class CatalogCustomerRequestFactory : ICustomerRequestFactory
{
    private readonly Random _random;
    private readonly int _customerPoolSize;
    private readonly IReadOnlyList<CatalogProduct> _catalog;

    public CatalogCustomerRequestFactory(
        IReadOnlyList<CatalogProduct>? catalog = null,
        int customerPoolSize = 500,
        int? seed = null)
    {
        _catalog = catalog is { Count: > 0 } ? catalog : ProductCatalogLoader.Load();
        _customerPoolSize = customerPoolSize;
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public CustomerRequest Create()
    {
        var customerId = $"cust-{_random.Next(1, _customerPoolSize + 1)}";
        var product = _catalog[_random.Next(_catalog.Count)];

        // Don't request more than what's actually in stock for that product — keeps
        // generated load representative of requests a real shopper could make.
        var maxQty = Math.Max(1, Math.Min(4, product.Quantity));
        var quantity = _random.Next(1, maxQty + 1).ToString();

        return new CustomerRequest
        {
            RequestId = Guid.NewGuid(),
            CustomerId = customerId,
            Parameters = new Dictionary<string, string>
            {
                ["productId"] = product.Id.ToString(),
                ["category"] = product.Category,
                ["qty"] = quantity
            }
        };
    }
}

/// <summary>
/// Negative/adversarial-input request factory for reliability and injection-resistance
/// testing (Task 7 quality testing). This does NOT attack anything — it only generates
/// malformed/boundary-pushing payloads (SQL-metacharacter strings, oversized values,
/// negative/absurd quantities, non-numeric IDs) and sends them down the exact same
/// <see cref="CustomerRequest"/> -&gt; <see cref="Endpoints.ICustomerEndpointClient"/> path as
/// normal traffic. Run against the simulated client it's a no-op; run against P1's real
/// HTTP endpoint it verifies the parameterised-query data-access layer (Req 8) and the
/// exception-handling/logging path (Req 6) reject bad input safely instead of crashing.
/// </summary>
public sealed class AdversarialCustomerRequestFactory : ICustomerRequestFactory
{
    private readonly Random _random;

    private static readonly string[] MaliciousProductIds =
    {
        "1 OR 1=1",
        "1; DROP TABLE products;--",
        "' UNION SELECT * FROM customers--",
        "<script>alert(1)</script>",
        "NaN",
        "999999999999999999999999",
        ""
    };

    private static readonly string[] MaliciousQuantities =
    {
        "-1",
        "0",
        "99999999",
        "1 OR '1'='1'",
        "abc"
    };

    public AdversarialCustomerRequestFactory(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public CustomerRequest Create()
    {
        var productId = MaliciousProductIds[_random.Next(MaliciousProductIds.Length)];
        var quantity = MaliciousQuantities[_random.Next(MaliciousQuantities.Length)];

        return new CustomerRequest
        {
            RequestId = Guid.NewGuid(),
            // A malicious/malformed customer identifier is part of the same negative-input
            // surface — e.g. an oversized string probing for a buffer/field-length issue.
            CustomerId = "cust-' OR '1'='1",
            Parameters = new Dictionary<string, string>
            {
                ["productId"] = productId,
                ["qty"] = quantity
            }
        };
    }
}
