using Neo4j.Driver;
using Neocore.Common;
using Neocore.Filters;
using Neocore.Models;
using Neocore.ViewModels;
using System.Text;

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
    
    public async Task Delete(int id)
    {
        const string query = @"
            MATCH (oldC:Contract {id: $id})
            DETACH DELETE oldC
        ";
        
        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    public async Task Update(int id, Contract contract)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(Contract)} (id: {id})
        ");

        await Delete(id);
        await Add(contract);
    }

    public async Task Add(Contract contract)
    {
        int id = await NewId();

        var query = new StringBuilder(@" 
            CREATE (c:Contract {id: $id, deliveryDate: $deliveryDate})
        "); // TODO aliases

        var parameters = new Dictionary<string, object>()
        {
            ["id"] = id,
            ["deliveryDate"] = contract.DeliveryDate ?? new LocalDate(2024, 01, 01),
        };

        int? vendorId = contract.Vendor?.Id;

        if (vendorId is not null)
        {
            query.Append(@"
                WITH c
                MATCH (v:Vendor {id: $vendorId})
                MERGE (c)-[:SIGNED_WITH]->(v)
            ");
            parameters["vendorId"] = vendorId;
        }

        var items = contract.Items?.Select(iwq =>
            new Dictionary<string, int?> 
            { 
                ["itemId"] = iwq.Item?.Id, 
                ["quantity"] = iwq.Quantity 
            }
        ).ToList();

        if (items?.Count > 0)
        {
            query.Append(@"
                WITH c
                UNWIND $items AS item
                MATCH (i:Item {id: item.itemId})
                MERGE (i)-[su:SUPPLIED_UNDER]->(c)
                SET su.quantity = item.quantity
            ");
            parameters["items"] = items;
        }

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    private async Task<int> NewId()
    {
        const string query = @$"
            MATCH ({Aliases.Contract}:Contract) 
            RETURN {Aliases.Contract}.id as id
            ORDER BY {Aliases.Contract}.id desc 
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }
}
