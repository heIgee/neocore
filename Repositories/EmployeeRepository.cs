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

    public async Task<Employee?> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Al.Employee}:Employee)")
            .Where($"{Al.Employee}.id = $id", "id", id)
            .Return($"{Al.Employee}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            Employee.FromRecord
        );
    }
}
