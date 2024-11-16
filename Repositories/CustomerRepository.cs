using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;

namespace Neocore.Repositories;

public class CustomerRepository(IDriver driver) : NeocoreRepository(driver)
{
    public async Task<IEnumerable<Customer>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Al.Customer}:Customer)")
            .Return($"{Al.Customer}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            Customer.FromRecord
        );
    }

    public async Task<Customer?> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Al.Customer}:Customer)")
            .Where($"{Al.Customer}.id = $id", "id", id)
            .Return($"{Al.Customer}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            Customer.FromRecord
        );
    }

    
    public async Task Update(int id, Customer customer)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(Customer)} (id: {id})
        ");

        const string query = @$"
            MATCH ({Al.Customer}:Customer {{id: $id}})
            SET {Al.Customer}.fullName = $fullName
        ";

        var parameters = GetParams(id, customer);

        await ExecuteWriteSingleAsync(
            query,
            parameters
        );
    }

    public async Task Add(Customer customer)
    {
        int id = await NewId();

        var query = @$"
            CREATE ({Al.Customer}:Customer {{id: $id, fullName: $fullName }})
        ";

        var parameters = GetParams(id, customer);

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Delete(int id)
    {
        const string query = @$"
            MATCH ({Al.Customer}:Customer {{id: $id}})
            DETACH DELETE {Al.Customer}
        ";
        
        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    private async Task<int> NewId()
    {
        const string query = @$"
            MATCH ({Al.Customer}:Customer)
            RETURN {Al.Customer}.id as id
            ORDER BY {Al.Customer}.id desc
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }

    private static Dictionary<string, object?> GetParams(int id, Customer employee) => new()
    {
        ["id"] = id,
        ["fullName"] = employee.FullName ?? string.Empty,
    };
}
