using Neo4j.Driver;
using Neocore.Common;
using Neocore.Filters;
using Neocore.Models;
using Neocore.Repositories.Abstract;
using Neocore.ViewModels;
using System.Text;

namespace Neocore.Repositories;

public class SaleRepository(IDriver driver) : NeocoreRepository(driver), ISaleRepository
{
    public async Task<IEnumerable<Sale>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Al.Sale}:Sale)")
            .Return($"{Al.Sale}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            Sale.FromRecord
        );
    }

    public async Task<Sale?> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Al.Sale}:Sale)")
            .Where($"{Al.Sale}.id = $id", "id", id)
            .Return($"{Al.Sale}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            Sale.FromRecord
        );
    }

    public async Task<SaleExtended?> FindExtendedById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Al.Sale}:Sale)")
            .Where($"{Al.Sale}.id = $id", "id", id)
            .OptionalMatch($"({Al.Sale})-[:SOLD_BY]->({Al.Employee}:Employee)")
            .OptionalMatch($"({Al.Sale})-[:ORDERED_BY]->({Al.Customer}:Customer)")
            .OptionalMatch($"({Al.Sale})-[inc:INCLUDES]->({Al.Item}:Item)")
            .With($"{Al.Sale}, {Al.Employee}, {Al.Customer}, " +
              $"collect({{item: {Al.Item}, quantity: inc.quantity, warrantyTerms: inc.warrantyTerms}}) as {Al.SoldItemList}")
            .Return($"{Al.Sale}, {Al.Employee}, {Al.Customer}, {Al.SoldItemList}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            SaleExtended.FromRecord
        );
    }

    public async Task<IEnumerable<SaleExtended>> FindExtendedByFilter(SaleFilter filter)
    {
        var builder = new QueryBuilder()
        .Match($"({Al.Sale}:Sale)")
        .OptionalMatch($"({Al.Sale})-[:SOLD_BY]->({Al.Employee}:Employee)")
        .OptionalMatch($"({Al.Sale})-[:ORDERED_BY]->({Al.Customer}:Customer)")
        .OptionalMatch($"({Al.Sale})-[inc:INCLUDES]->({Al.Item}:Item)")
        .With($"{Al.Sale}, {Al.Employee}, {Al.Customer}, " +
              $"collect({{item: {Al.Item}, quantity: inc.quantity, warrantyTerms: inc.warrantyTerms}}) as {Al.SoldItemList}");

        filter.Apply(builder);

        builder.Return($"{Al.Sale}, {Al.Employee}, {Al.Customer}, {Al.SoldItemList}");

        var (query, parameters) = builder.Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            SaleExtended.FromRecord
        );
    }

    public async Task Update(int id, SaleExtended saleX)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(SaleExtended)} (id: {id})
        ");

        await Delete(id);
        await Add(saleX, id);
    }

    public async Task Add(SaleExtended saleX, int? oldId = null)
    {
        if (saleX.Sale is null) return;

        int id = oldId ?? await NewId();

        var query = new StringBuilder(@"
            CREATE (s:Sale {id: $id, total: $total, date: $date, isBuild: $isBuild})
        ");

        var parameters = GetParams(id, saleX.Sale);

        int? employeeId = saleX.Employee?.Id;

        if (employeeId is not null)
        {
            query.Append(@"
                WITH s
                MATCH (e:Employee {id: $employeeId})
                MERGE (s)-[:SOLD_BY]->(e)
            ");
            parameters["employeeId"] = employeeId;
        }

        int? customerId = saleX.Customer?.Id;

        if (customerId is not null)
        {
            query.Append(@"
                WITH s
                MATCH (c:Customer {id: $customerId})
                MERGE (s)-[:ORDERED_BY]->(c)
            ");
            parameters["customerId"] = customerId;
        }

        var items = saleX.Items?.Select(soldItem =>
            new Dictionary<string, object?>
            {
                ["itemId"] = soldItem.Item?.Id,
                ["quantity"] = soldItem.Quantity,
                ["warrantyTerms"] = soldItem.WarrantyTerms
            }
        ).ToList();

        if (items?.Count > 0)
        {
            query.Append(@"
                WITH s
                UNWIND $items AS item
                MATCH (i:Item {id: item.itemId})
                MERGE (i)<-[su:INCLUDES]-(s)
                SET su.quantity = item.quantity,
                    su.warrantyTerms = item.warrantyTerms
            ");
            parameters["items"] = items;
        }

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Delete(int id)
    {
        const string query = @"
            MATCH (s:Sale {id: $id})
            DETACH DELETE s
        ";

        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    private async Task<int> NewId()
    {
        const string query = @"
            MATCH (s:Sale)
            RETURN s.id as id
            ORDER BY s.id desc
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }

    private static Dictionary<string, object> GetParams(int id, Sale sale) => new()
    {
        ["id"] = id,
        ["total"] = sale.Total ?? 0.0f,
        ["date"] = sale.Date ?? new LocalDate(2024, 01, 01),
        ["isBuild"] = sale.IsBuild ?? false,
    };
}
