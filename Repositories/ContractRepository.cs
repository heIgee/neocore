using Neo4j.Driver;
using Neocore.Common;
using Neocore.Filters;
using Neocore.Models;
using Neocore.Repositories.Abstract;
using Neocore.ViewModels;
using System.Text;

namespace Neocore.Repositories;

public class ContractRepository(IDriver driver) : NeocoreRepository(driver), IContractRepository
{
    public async Task<ContractExtended?> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Al.Contract}:Contract)")
            .Where($"{Al.Contract}.id = $id", "id", id)
            .OptionalMatch($"({Al.Item}:Item)-[su:SUPPLIED_UNDER]->({Al.Contract})")
            .OptionalMatch($"({Al.Contract})-[:SIGNED_WITH]->({Al.Vendor}:Vendor)")
            .Return($"{Al.Contract}, {Al.Vendor}, " +
                $"COLLECT({{item: {Al.Item}, quantity: su.quantity}}) as {Al.ItemWithQuantityList}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            ContractExtended.FromRecord
        );
    }

    public async Task<IEnumerable<ContractExtended>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Al.Contract}:Contract)")
            .Return($"{Al.Contract}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            ContractExtended.FromRecord
        );
    }

    public async Task<IEnumerable<ContractExtended>> FindByFilter(ContractFilter filter)
    {
        var builder = new QueryBuilder()
            .Match($"({Al.Contract}:Contract)")
            .OptionalMatch($"({Al.Contract})-[:SIGNED_WITH]->({Al.Vendor}:Vendor)");

        filter.Apply(builder);

        builder.Return($" DISTINCT {Al.Contract}, {Al.Vendor}");

        var (query, parameters) = builder.Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            ContractExtended.FromRecord
        );
    }

    public async Task<IEnumerable<ContractSummary>> FindByFilterWithSummary(ContractFilter filter)
    {
        var builder = new QueryBuilder()
            //.Match($"({Al.ContractExtended}:ContractExtended)")
            //.OptionalMatch($"({Al.ContractExtended})-[:SIGNED_WITH]->()");
            .Match($"({Al.Contract}:Contract)-[:SIGNED_WITH]->({Al.Vendor}:Vendor)");
            //.OptionalMatch($"({Al.ContractExtended})<-[r:SUPPLIED_UNDER]-({Al.Item}:Item)");

        filter.Apply(builder);

        builder.Return($" DISTINCT {Al.Contract}, {Al.Vendor}");

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
            MATCH (c:Contract {id: $id})
            DETACH DELETE c
        ";
        
        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    public async Task Update(int id, ContractExtended contract)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(ContractExtended)} (id: {id})
        ");

        await Delete(id);
        await Add(contract, id);
    }

    public async Task Add(ContractExtended contract, int? oldId = null)
    {
        int id = oldId ?? await NewId();

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
            MATCH (c:Contract)
            RETURN c.id as id
            ORDER BY c.id desc
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }
}
