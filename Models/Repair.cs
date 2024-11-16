using Neo4j.Driver;
using Neocore.Common;

namespace Neocore.Models;

public class Repair : INeocoreNode<Repair>
{
    public int? Id { get; set; }
    public RepairStatus? Status { get; set; }
    public bool? IsWarranty { get; set; }
    public string? Cause { get; set; }
    public float? Price { get; set; }
    public LocalDate? HandedDate { get; set; }
    public LocalDate? ReturnedDate { get; set; }

    public static Repair FromRecord(IRecord record)
    {
        var node = record[Al.Repair].As<INode>();
        return node is null ? new() : FromNode(node);
    }

    public static Repair FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Status = Enum.Parse<RepairStatus>(
            node.Properties["status"].As<string>(), 
            ignoreCase: true
        ),
        IsWarranty = node.Properties["isWarranty"].As<bool>(),
        Cause = node.Properties["cause"].As<string>(),
        Price = node.Properties["price"].As<float>(),
        HandedDate = node.Properties["handedDate"].As<LocalDate>(),
        ReturnedDate = node.Properties.ContainsKey("returnedDate") 
            ? node.Properties["returnedDate"].As<LocalDate>() 
            : null,
    };
}
