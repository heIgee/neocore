using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;
using Neocore.Repositories.Abstract;

namespace Neocore.Repositories;

public class UserRepository(IDriver driver) : NeocoreRepository(driver), IUserRepository
{
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
}
