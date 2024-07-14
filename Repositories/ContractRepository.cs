using Neo4j.Driver;
using Neocore.Common;
using Neocore.Components.Pages;
using Neocore.Filters;
using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories;

public class ContractRepository(IDriver driver) : NeocoreRepository(driver), IContractRepository
{
    public async Task<Contract?> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Aliases.Contract}:Contract)")
            .Where($"{Aliases.Contract}.id = $id", "id", id)
            .OptionalMatch($"({Aliases.Item}:Item)-[su:SUPPLIED_UNDER]->({Aliases.Contract})")
            .OptionalMatch($"({Aliases.Contract})-[:SIGNED_WITH]->({Aliases.Vendor}:Vendor)")
            .Return($"{Aliases.Contract}, {Aliases.Vendor}, " +
                $"COLLECT({{item: {Aliases.Item}, quantity: su.quantity}}) as {Aliases.ItemWithQuantityList}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            Contract.FromRecord
        );
    }

    public async Task<IEnumerable<Contract>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Aliases.Contract}:Contract)")
            .Return($"{Aliases.Contract}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            Contract.FromRecord
        );
    }

    public async Task<IEnumerable<Contract>> FindByFilter(ContractFilter filter)
    {
        var builder = new QueryBuilder()
            .Match($"({Aliases.Contract}:Contract)")
            .OptionalMatch($"({Aliases.Contract})-[:SIGNED_WITH]->({Aliases.Vendor}:Vendor)");

        filter.Apply(builder);

        builder.Return($" DISTINCT {Aliases.Contract}, {Aliases.Vendor}");

        var (query, parameters) = builder.Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            Contract.FromRecord
        );
    }

    public async Task<IEnumerable<ContractSummary>> FindByFilterWithSummary(ContractFilter filter)
    {
        var builder = new QueryBuilder()
            //.Match($"({Aliases.Contract}:Contract)")
            //.OptionalMatch($"({Aliases.Contract})-[:SIGNED_WITH]->({Aliases.Vendor}:Vendor)");
            .Match($"({Aliases.Contract}:Contract)-[:SIGNED_WITH]->({Aliases.Vendor}:Vendor)");
            //.OptionalMatch($"({Aliases.Contract})<-[r:SUPPLIED_UNDER]-({Aliases.Item}:Item)");

        filter.Apply(builder);

        builder.Return($"DISTINCT {Aliases.Contract}, {Aliases.Vendor}");

        var (query, parameters) = builder.Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            ContractSummary.FromRecord
        );
    }   
    
    public async Task Update(int id, Contract contract)
    {
        var query = @"
            MATCH (oldC:Contract {id: $id})
            DETACH DELETE oldC
            CREATE (c:Contract {id: $id, deliveryDate: $deliveryDate})
            WITH c
            MATCH (v:Vendor {id: $vendorId})
            MERGE (c)-[:SIGNED_WITH]->(v)
            WITH c
            UNWIND $items AS item
            MATCH (i:Item {id: item.itemId})
            MERGE (i)-[su:SUPPLIED_UNDER]->(c)
            SET su.quantity = item.quantity
            RETURN 0
        ";

        //RETURN c, v, collect({item: i, quantity: su.quantity}) as iql;

        var parameters = new
        {
            id,
            deliveryDate = contract.DeliveryDate,
            vendorId = contract.Vendor?.Id,
            items = contract.Items?.Select(iwq => 
                new { itemId = iwq.Item?.Id, quantity = iwq.Quantity }
            ).ToList()
        };

        await ExecuteWriteSingleAsync(
            query,
            parameters
            //Contract.FromRecord
        );
    }
}
