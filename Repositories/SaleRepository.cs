using Microsoft.AspNetCore.Http;
using Neo4j.Driver;
using Neocore.Common;
using Neocore.Filters;
using Neocore.Models;
using Neocore.ViewModels;
using System.Text;

namespace Neocore.Repositories;

public class SaleRepository(IDriver driver) : NeocoreRepository(driver)
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

    public async Task Update(int id, Sale sale)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(Sale)} (id: {id})
        ");

        var query = new StringBuilder(@$" 
            MATCH ({Al.Sale}:Sale {{id: $id}})
            SET {Al.Sale}.name = $name, 
                {Al.Sale}.role = $role, 
                {Al.Sale}.password = $password
        "); 

        object parameters = GetParams(id, sale);

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Add(Sale sale)
    {
        int id = await NewId();

        var query = new StringBuilder(@$" 
            CREATE ({Al.Sale}:Sale {{id: $id, total: $total, date: $date }})
        ");

        object parameters = GetParams(id, sale);

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

    private static object GetParams(int id, Sale sale) => new
    {
        id,
        total = sale.Total ?? 0.0f,
        date = sale.Date ?? new LocalDate(2024, 01, 01),
    };
}
