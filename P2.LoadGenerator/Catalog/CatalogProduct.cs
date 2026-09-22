namespace P2.LoadGenerator.Catalog;

/// <summary>
/// Mirrors P1's <c>products</c> table shape (id, name, category, quantity, price) so load
/// requests can reference real product IDs/categories/prices instead of fake SKU strings.
/// Deliberately a plain copy of the shape rather than a shared reference to P1's own
/// <c>Product</c> class — P2 stays buildable and testable without a project reference to
/// P1 or a live Postgres connection, matching how the two modules are still developed
/// independently (see mid-project report, shared data contract note).
/// </summary>
public sealed record CatalogProduct
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required int Quantity { get; init; }
    public required decimal Price { get; init; }
}
