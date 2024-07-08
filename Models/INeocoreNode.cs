using Neo4j.Driver;

namespace Neocore.Models;

public interface INeocoreNode<T>
{
    int Id { get; set; }
    static abstract T FromNode(IEntity node);
}