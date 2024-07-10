using Neo4j.Driver;
using Neocore.Common;

namespace Neocore.Models;

public class Contract : INeocoreNode<Contract>
{
    public int Id { get; set; }
    public LocalDate? DeliveryDate { get; set; }

    public static Contract FromRecord(IRecord record) => FromNode(record[Aliases.Contract].As<INode>());

    public static Contract FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        DeliveryDate = node.Properties["deliveryDate"].As<LocalDate>()
    };
}
