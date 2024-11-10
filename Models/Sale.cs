using Neo4j.Driver;
using Neocore.Common;

namespace Neocore.Models;

public class Sale : INeocoreNode<Sale>
{
    public int? Id { get; set; }
    public float? Total { get; set; }
    public LocalDate? Date { get; set; }

    public static Sale FromRecord(IRecord record)
    {
        var node = record[Al.Sale].As<INode>();
        return node is null ? new() : FromNode(node);
    }

    public static Sale FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Total = node.Properties["total"].As<float>(),
        Date = node.Properties["date"].As<LocalDate>(),
    };
}
