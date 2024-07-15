using Neo4j.Driver;

namespace Neocore.Repositories;

public abstract class NeocoreRepository(IDriver driver)
{
    private readonly IDriver driver = driver;
    protected IDriver Driver => driver;

    protected async Task<T?> ExecuteReadSingleAsync<T>(string query, object parameters, Func<IRecord, T> mapper)
    {
        await using var session = Driver.AsyncSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, parameters);
            var res = await cursor.ToListAsync(mapper);

            return res.Count switch
            {
                0 => default,
                1 => res[0],
                _ => throw new InvalidOperationException($"Multiple results found, only one was expected. Query: {query}")
            };
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

    protected async Task ExecuteWriteSingleAsync(string query, object parameters)
    {
        await using var session = Driver.AsyncSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            Console.WriteLine(parameters);
            var cursor = await tx.RunAsync(query, parameters);
        });
    }
}
