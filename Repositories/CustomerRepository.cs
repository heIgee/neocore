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
}
