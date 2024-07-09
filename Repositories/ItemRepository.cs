using Neo4j.Driver;
using Neocore.Models;

namespace Neocore.Repositories;

public class ItemRepository(IDriver driver) : NeocoreRepository(driver), IItemRepository
{
    public async Task<Item> FindById(int id)
    {
        const string query = "MATCH (i:Item {id: $id}) RETURN i";
        return await ExecuteReadSingleAsync(
            query,
            new { id },
            record => Item.FromNode(record["i"].As<INode>())
        );
    }

    public async Task<IEnumerable<Item>> FindAll()
    {
        const string query = "MATCH (i:Item) RETURN i";
        return await ExecuteReadListAsync(
            query,
            new { },
            record => Item.FromNode(record["i"].As<INode>())
        );
    }

    public async Task<IEnumerable<Item>> FindByVendor(int vendorId)
    {
        const string query = @"
            MATCH (v:Vendor {id: $vendorId})<-[:SIGNED_WITH]-(c:Contract)<-[su:SUPPLIED_UNDER]-(i:Item) 
            RETURN DISTINCT i
        ";
        return await ExecuteReadListAsync(
            query,
            new { vendorId },
            record => Item.FromNode(record["i"].As<INode>())
        );
    }
}
