using Neo4j.Driver;

namespace Neocore.Models;

public interface INeocoreNode<T>
{
    int Id { get; set; }
    static abstract T FromRecord(IRecord record);
    static abstract T FromNode(INode node);
}