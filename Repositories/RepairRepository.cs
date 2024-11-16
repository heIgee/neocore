using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;
using Neocore.ViewModels;
using System.Text;

namespace Neocore.Repositories;

public class RepairRepository(IDriver driver) : NeocoreRepository(driver)
{
    public async Task<IEnumerable<Repair>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Al.Repair}:Repair)")
            .Return($"{Al.Repair}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            Repair.FromRecord
        );
    }

    public async Task<IEnumerable<RepairExtended>> FindAllExtended()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Al.Repair}:Repair)")
            .OptionalMatch($"({Al.Repair})-[:HANDLED_BY]->({Al.Employee}:Employee)")
            .OptionalMatch($"({Al.Repair})-[:REQUESTED_BY]->({Al.Customer}:Customer)")
            .OptionalMatch($"({Al.Repair})-[:INVOLVES]->({Al.Item}:Item)")
            .Return($"{Al.Repair}, {Al.Employee}, {Al.Customer}, {Al.Item}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            RepairExtended.FromRecord
        );
    }

    public async Task<Repair?> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Al.Repair}:Repair)")
            .Where($"{Al.Repair}.id = $id", "id", id)
            .Return($"{Al.Repair}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            Repair.FromRecord
        );
    }

    public async Task<RepairExtended?> FindExtendedById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Al.Repair}:Repair)")
            .Where($"{Al.Repair}.id = $id", "id", id)
            .OptionalMatch($"({Al.Repair})-[:HANDLED_BY]->({Al.Employee}:Employee)")
            .OptionalMatch($"({Al.Repair})-[:REQUESTED_BY]->({Al.Customer}:Customer)")
            .OptionalMatch($"({Al.Repair})-[:INVOLVES]->({Al.Item}:Item)")
            .Return($"{Al.Repair}, {Al.Employee}, {Al.Customer}, {Al.Item}")
            .Build();

        Console.WriteLine(query);

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            RepairExtended.FromRecord
        );
    }

    //public async Task<IEnumerable<RepairExtended>> FindExtendedByFilter(SaleFilter filter)
    //{
    //    var builder = new QueryBuilder()
    //    .Match($"({Al.Sale}:Repair)")
    //    .OptionalMatch($"({Al.Sale})-[:SOLD_BY]->({Al.Employee}:Employee)")
    //    .OptionalMatch($"({Al.Sale})-[:ORDERED_BY]->({Al.Customer}:Customer)")
    //    .OptionalMatch($"({Al.Sale})-[inc:INCLUDES]->({Al.Item}:Item)")
    //    .With($"{Al.Sale}, {Al.Employee}, {Al.Customer}, " +
    //          $"collect({{item: {Al.Item}, quantity: inc.quantity, warrantyTerms: inc.warrantyTerms}}) as {Al.SoldItemList}");

    //    filter.Apply(builder);
    
    //    builder.Return($"{Al.Sale}, {Al.Employee}, {Al.Customer}, {Al.SoldItemList}");
    
    //    var (query, parameters) = builder.Build();
    //    Console.WriteLine(query);

    //    return await ExecuteReadListAsync(
    //        query,
    //        parameters,
    //        SaleExtended.FromRecord
    //    );
    //}

    public async Task Update(int id, RepairExtended repairX)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(RepairExtended)} (id: {id})
        ");

        await Delete(id);
        await Add(repairX, id);
    }

    public async Task Add(RepairExtended repairX, int? oldId = null)
    {
        if (repairX.Repair is null) return;

        int id = oldId ?? await NewId();

        var query = new StringBuilder(@$"
            CREATE ({Al.Repair}:Repair {{id: $id, status: $status, isWarranty: $isWarranty, cause: $cause, 
                price: $price, handedDate: $handedDate, returnedDate: $returnedDate 
            }})
        ");

        var parameters = GetParams(id, repairX.Repair);

        int? employeeId = repairX.Employee?.Id;

        if (employeeId is not null)
        {
            query.Append(@$"
                WITH {Al.Repair}
                MATCH (e:Employee {{id: $employeeId}})
                MERGE ({Al.Repair})-[:HANDLED_BY]->(e)
            ");
            parameters["employeeId"] = employeeId;
        }

        int? customerId = repairX.Customer?.Id;

        if (customerId is not null)
        {
            query.Append(@$"
                WITH {Al.Repair}
                MATCH (c:Customer {{id: $customerId}})
                MERGE ({Al.Repair})-[:REQUESTED_BY]->(c)
            ");
            parameters["customerId"] = customerId;
        }

        int? itemId = repairX.Item?.Id;

        if (itemId is not null)
        {
            query.Append(@$"
                WITH {Al.Repair}
                MATCH (i:Item {{id: $itemId}})
                MERGE ({Al.Repair})-[:INVOLVES]->(i)
            ");
            parameters["itemId"] = itemId;
        }

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Delete(int id)
    {
        const string query = @$"
            MATCH ({Al.Repair}:Repair {{id: $id}})
            DETACH DELETE {Al.Repair}
        ";
        
        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    private async Task<int> NewId()
    {
        const string query = @$"
            MATCH ({Al.Repair}:Repair)
            RETURN {Al.Repair}.id as id
            ORDER BY {Al.Repair}.id desc
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }

    private static Dictionary<string, object?> GetParams(int id, Repair repair) => new()
    {
        ["id"] = id,
        ["status"] = repair.Status.HasValue
            ? repair.Status.Value.ToString()
            : RepairStatus.HandedOver.ToString(),
        ["isWarranty"] = repair.IsWarranty ?? false,
        ["cause"] = repair.Cause ?? string.Empty,
        ["price"] = repair.Price ?? 0.0f,
        ["handedDate"] = repair.HandedDate ?? new LocalDate(2024, 01, 01),
        ["returnedDate"] = repair.ReturnedDate,
    };
}
