using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;
using Neocore.Repositories.Abstract;
using System.Text;

namespace Neocore.Repositories;

public class UserRepository(IDriver driver) : NeocoreRepository(driver), IUserRepository
{
    public async Task<IEnumerable<User>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Aliases.User}:User)")
            .Return($"{Aliases.User}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            User.FromRecord
        );
    }

    public async Task<User?> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Aliases.User}:User)")
            .Where($"{Aliases.User}.id = $id", "id", id)
            .Return($"{Aliases.User}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            User.FromRecord
        );
    }

    public async Task<User?> FindByUsername(string username)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Aliases.User}:User)")
            .Where($"{Aliases.User}.name = $username", "username", username)
            .Return($"{Aliases.User}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            User.FromRecord
        );
    }

    public async Task<User?> ValidateUser(string username, string password)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Aliases.User}:User)")
            .Where($"{Aliases.User}.name = $username", "username", username)
            .Where($"{Aliases.User}.password = $password", "password", password)
            .Return($"{Aliases.User}")
            .Build();

        return await ExecuteReadSingleAsync(
            query,
            parameters,
            User.FromRecord
        );
    }

    public async Task Update(int id, User user)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(User)} (id: {id})
        ");

        var query = new StringBuilder(@$" 
            MATCH ({Aliases.User}:User {{id: $id}})
            SET {Aliases.User}.name = $name, 
                {Aliases.User}.role = $role, 
                {Aliases.User}.password = $password
        "); 

        object parameters = new
        {
            id,
            name = user.Name ?? string.Empty,
            role = user.Role.HasValue 
                ? user.Role.Value.ToString().ToLower() 
                : UserRole.Viewer.ToString().ToLower(),
            password = user.Password ?? string.Empty,
        };

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Add(User user)
    {
        int id = await NewId();

        var query = new StringBuilder(@$" 
            CREATE ({Aliases.User}:User {{id: $id, name: $name, role: $role, password: $password}})
        ");

       object parameters = new
        {
            id,
            name = user.Name ?? string.Empty,
            role = user.Role.HasValue 
                ? user.Role.Value.ToString().ToLower() 
                : UserRole.Viewer.ToString().ToLower(),
            password = user.Password ?? string.Empty,
        }; // TODO: get params reusable method

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Delete(int id)
    {
        const string query = @"
            MATCH (u:User {id: $id})
            DETACH DELETE u
        ";
        
        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    private async Task<int> NewId()
    {
        const string query = @"
            MATCH (u:User)
            RETURN u.id as id
            ORDER BY u.id desc
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }
}
