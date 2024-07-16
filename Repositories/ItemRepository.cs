using Neo4j.Driver;
using Neocore.Common;
using Neocore.Filters;
using Neocore.Models;
using System.Text;

namespace Neocore.Repositories;

public class ItemRepository(IDriver driver) : NeocoreRepository(driver), IItemRepository
{
    public async Task<Item?> FindById(int id)
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
            .Match($"({Aliases.Item}:Item)")
            .OptionalMatch($"({Aliases.Vendor}:Vendor)<-[:SIGNED_WITH]-(Contract)<-[:SUPPLIED_UNDER]-({Aliases.Item})");
        //.Match($"({Aliases.Item}:Item)-[:SUPPLIED_UNDER]->(Contract)-[:SIGNED_WITH]->({Aliases.Vendor}:Vendor)"); // wwww

        filter.Apply(builder);

        //builder.OptionalMatch($"({Aliases.Vendor}:Vendor)<-[:SIGNED_WITH]-(Contract)<-[:SUPPLIED_UNDER]-({Aliases.Item})");

        builder.Return($"DISTINCT {Aliases.Item}");

        var (query, parameters) = builder.Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            Item.FromRecord
        );
    }

    public async Task Delete(int id)
    {
        const string query = @"
            MATCH (i:Item {id: $id})
            DETACH DELETE i
        ";
        
        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    public async Task Update(int id, Item item)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(Item)} (id: {id})
        ");

        var query = new StringBuilder(@" 
            MATCH (i:Item {id: $id})
            SET i.name = $name, i.type = $type, i.manufacturer = $manufacturer,
                i.specifications = $specifications, i.price = $price
        "); // TODO aliases

        object parameters = new
        {
            id,
            name = item.Name ?? string.Empty,
            type = item.Type ?? string.Empty,
            manufacturer = item.Manufacturer ?? string.Empty,
            specifications = item.Specifications ?? string.Empty,
            price = item.Price ?? 0.0f
        };

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Add(Item item)
    {
        int id = await NewId();

        var query = new StringBuilder(@" 
            CREATE (i:Item {id: $id, name: $name, type: $type, 
                manufacturer: $manufacturer, specifications: $specifications, 
                price: $price})
        "); // TODO aliases

        object parameters = new
        {
            id,
            name = item.Name ?? string.Empty,
            type = item.Type ?? string.Empty,
            manufacturer = item.Manufacturer ?? string.Empty,
            specifications = item.Specifications ?? string.Empty,
            price = item.Price ?? 0.0f
        };

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    private async Task<int> NewId()
    {
        const string query = @$"
            MATCH (i:Item)
            RETURN i.id as id
            ORDER BY i.id desc
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }
}
