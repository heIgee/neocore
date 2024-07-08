using Neo4j.Driver;
using Neocore.Models;

namespace Neocore.Repositories;

public class ItemRepository(IDriver driver) : IItemRepository
{
    private readonly IDriver _driver = driver;

    public async Task<Item> FindById(int id)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (i:Item {id: $id}) RETURN i", new { id });
            return await cursor.SingleAsync(record => MapToItem(record["i"].As<INode>()));
        });
        return result;
    }

    public async Task<IEnumerable<Item>> FindAll()
    {
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (i:Item) RETURN i");
            return await cursor.ToListAsync(record => MapToItem(record["i"].As<INode>()));
        });
        return result;
    }

    private static Item MapToItem(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Name = node.Properties["name"].As<string>(),
        Type = node.Properties["type"].As<string>(),
        Manufacturer = node.Properties["manufacturer"].As<string>(),
        Specifications = node.Properties["specifications"].As<string>(),
        Price = node.Properties["price"].As<float>()
    };
}
