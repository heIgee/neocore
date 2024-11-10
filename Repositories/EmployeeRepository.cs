using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;

namespace Neocore.Repositories;

public class EmployeeRepository(IDriver driver) : NeocoreRepository(driver)
{
    public async Task<IEnumerable<Employee>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Al.Employee}:Employee)")
            .Return($"{Al.Employee}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            Employee.FromRecord
        );
    }
}
