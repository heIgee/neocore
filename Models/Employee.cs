using Neo4j.Driver;
using Neocore.Common;

namespace Neocore.Models;

public class Employee : INeocoreNode<Employee>
{
    public int? Id { get; set; }
    public string? FullName { get; set; }

    public static Employee FromRecord(IRecord record)
    {
        var node = record[Al.Employee].As<INode>();
        return node is null ? new() : FromNode(node); 
    }

    public static Employee FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        FullName = node.Properties["fullName"].As<string>(),
    };
}
