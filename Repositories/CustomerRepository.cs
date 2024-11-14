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
}
