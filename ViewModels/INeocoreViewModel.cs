using Neo4j.Driver;

namespace Neocore.ViewModels;

public interface INeocoreViewModel<T>
{
    static abstract T FromRecord(IRecord record);
}
