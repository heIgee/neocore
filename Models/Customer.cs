using Neo4j.Driver;
using Neocore.Common;

namespace Neocore.Models;

public class Customer : INeocoreNode<Customer>
{
    public int? Id { get; set; }
    public string? FullName { get; set; }

    public static Customer FromRecord(IRecord record)
    {
        var node = record[Al.Customer].As<INode>();
        return node is null ? new() : FromNode(node); 
    }

    public static Customer FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        FullName = node.Properties["fullName"].As<string>(),
    };
}
