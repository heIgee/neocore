using Neo4j.Driver;
using Neocore.Common;
using Neocore.Filters;
using Neocore.Models;

namespace Neocore.Repositories;

public class ItemRepository(IDriver driver) : NeocoreRepository(driver), IItemRepository
{
    public async Task<Item> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Aliases.Item}:Item)")
            .Where($"{Aliases.Item}.id = $id", "id", id)
            .Return($"{Aliases.Item}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            Item.FromRecord
        );
    }

    public async Task<IEnumerable<Item>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Aliases.Item}:Item)")
            .Return($"{Aliases.Item}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            Item.FromRecord
        );
    }

    public async Task<IEnumerable<Item>> FindByVendor(int vendorId)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Aliases.Vendor}:Vendor)<-[:SIGNED_WITH]-(Contract)<-[:SUPPLIED_UNDER]-({Aliases.Item}:Item)")
            .Where($"({Aliases.Vendor}).id = $vendorId", "vendorId", vendorId)
            .Return($"DISTINCT {Aliases.Item}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            Item.FromRecord
        );
    }

    public async Task<IEnumerable<Item>> FindByFilter(ItemFilter filter)
    {
        var builder = new QueryBuilder()
            .Match($"({Aliases.Vendor}:Vendor)<-[:SIGNED_WITH]-(Contract)<-[:SUPPLIED_UNDER]-({Aliases.Item}:Item)");

        filter.Apply(builder);

        builder.Return($"DISTINCT {Aliases.Item}");

        var (query, parameters) = builder.Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            Item.FromRecord
        );
    }
}
