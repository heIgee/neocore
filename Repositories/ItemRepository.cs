using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;

namespace Neocore.Repositories;

public class ItemRepository(IDriver driver) : NeocoreRepository(driver), IItemRepository
{
    public async Task<Item> FindById(int id)
    {
        const string query = $"MATCH ({Aliases.Item}:Item {{id: $id}}) RETURN {Aliases.Item}";
        return await ExecuteReadSingleAsync(
            query,
            new { id },
            Item.FromRecord
        );
    }

    public async Task<IEnumerable<Item>> FindAll()
    {
        const string query = $"MATCH ({Aliases.Item}:Item) RETURN {Aliases.Item}";
        return await ExecuteReadListAsync(
            query,
            new { },
            Item.FromRecord
        );
    }

    public async Task<IEnumerable<Item>> FindByVendor(int vendorId)
    {
        const string query = @$"
            MATCH ({Aliases.Vendor}:Vendor {{id: $vendorId}})<-[:SIGNED_WITH]-(Contract)<-[su:SUPPLIED_UNDER]-({Aliases.Item}:Item) 
            RETURN DISTINCT {Aliases.Item}
        ";
        return await ExecuteReadListAsync(
            query,
            new { vendorId },
            Item.FromRecord
        );
    }
}
