using Neo4j.Driver;

namespace Neocore.Repositories;

public abstract class NeocoreRepository(IDriver driver)
{
    private readonly IDriver driver = driver;
    protected IDriver Driver => driver;

    protected async Task<T> ExecuteReadSingleAsync<T>(string query, object parameters, Func<IRecord, T> mapper)
    {
        await using var session = Driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            return await cursor.SingleAsync(mapper);
        });
    }

    protected async Task<IEnumerable<T>> ExecuteReadListAsync<T>(string query, object parameters, Func<IRecord, T> mapper)
    {
        await using var session = Driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            return await cursor.ToListAsync(mapper);
        });
    }
}
