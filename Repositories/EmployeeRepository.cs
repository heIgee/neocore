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

    public async Task Update(int id, Employee employee)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(Employee)} (id: {id})
        ");

        const string query = @$"
            MATCH ({Al.Employee}:Employee {{id: $id}})
            SET {Al.Employee}.fullName = $fullName
        ";

        var parameters = GetParams(id, employee);

        await ExecuteWriteSingleAsync(
            query,
            parameters
        );
    }

    public async Task Add(Employee employee)
    {
        int id = await NewId();

        var query = @$"
            CREATE ({Al.Employee}:Employee {{id: $id, fullName: $fullName }})
        ";

        var parameters = GetParams(id, employee);

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Delete(int id)
    {
        const string query = @$"
            MATCH ({Al.Employee}:Employee {{id: $id}})
            DETACH DELETE {Al.Employee}
        ";
        
        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    private async Task<int> NewId()
    {
        const string query = @$"
            MATCH ({Al.Employee}:Employee)
            RETURN {Al.Employee}.id as id
            ORDER BY {Al.Employee}.id desc
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }

    private static Dictionary<string, object?> GetParams(int id, Employee employee) => new()
    {
        ["id"] = id,
        ["fullName"] = employee.FullName ?? string.Empty,
    };
}
