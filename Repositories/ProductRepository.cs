using Neo4j.Driver;
using Neocore.Models;

namespace Neocore.Repositories;

public class ProductRepository(IDriver driver) : IProductRepository
{
    private readonly IDriver _driver = driver;

    public async Task<Product> FindById(int id)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (p:Product {id: $id}) RETURN p", new { id });
            return await cursor.SingleAsync(record => MapToProduct(record["p"].As<INode>()));
        });
        return result;
    }

    public async Task<IEnumerable<Product>> FindAll()
    {
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (p:Product) RETURN p");
            return await cursor.ToListAsync(record => MapToProduct(record["p"].As<INode>()));
        });
        return result;
    }

    private static Product MapToProduct(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Name = node.Properties["name"].As<string>(),
        Type = node.Properties["type"].As<string>(),
        Manufacturer = node.Properties["manufacturer"].As<string>(),
        Specifications = node.Properties["specifications"].As<string>(),
        Price = node.Properties["price"].As<float>()
    };
}
