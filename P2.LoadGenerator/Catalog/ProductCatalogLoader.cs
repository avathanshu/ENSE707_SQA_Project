using System.Text.Json;

namespace P2.LoadGenerator.Catalog;

/// <summary>
/// Loads the product catalogue used to build realistic <see cref="Execution.CustomerRequest"/>
/// instances. Reads <c>Catalog/data/products-seed.json</c> (copied to the output directory —
/// see the csproj's Content item), which was generated from the same category/pricing shape as
/// P1's real seed SQL, so load-test traffic exercises realistic product IDs and price bands
/// without requiring a live database connection.
/// </summary>
public static class ProductCatalogLoader
{
    private const string DefaultSeedFileName = "products-seed.json";

    /// <summary>A tiny built-in fallback so callers never fail just because the seed file
    /// wasn't copied next to the executable (e.g. a test host with a different output layout).</summary>
    private static readonly IReadOnlyList<CatalogProduct> FallbackCatalog = new[]
    {
        new CatalogProduct { Id = 1, Name = "Fallback Booster Pack", Category = "Trading Cards", Quantity = 50, Price = 9.99m },
        new CatalogProduct { Id = 2, Name = "Fallback Dice Set", Category = "Dice", Quantity = 50, Price = 14.99m },
        new CatalogProduct { Id = 3, Name = "Fallback Board Game", Category = "Board Games", Quantity = 50, Price = 39.99m },
    };

    public static IReadOnlyList<CatalogProduct> Load(string? filePath = null)
    {
        var path = filePath ?? Path.Combine(AppContext.BaseDirectory, "Catalog", "data", DefaultSeedFileName);

        if (!File.Exists(path))
        {
            return FallbackCatalog;
        }

        try
        {
            var json = File.ReadAllText(path);
            var products = JsonSerializer.Deserialize<List<CatalogProduct>>(json);
            return products is { Count: > 0 } ? products : FallbackCatalog;
        }
        catch (JsonException)
        {
            // Malformed seed file shouldn't take down a load test run — fall back quietly.
            return FallbackCatalog;
        }
    }
}
