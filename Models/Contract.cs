using Neo4j.Driver;
using Neocore.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Neocore.Models;

public class Contract : INeocoreNode<Contract>
{
    public int? Id { get; set; }
    public LocalDate? DeliveryDate { get; set; }

    public Vendor? Vendor { get; set; }
    public List<ItemWithQuantity>? Items { get; set; }


    public static Contract FromRecord(IRecord record)
    {
        var node = record[Aliases.Contract].As<INode>();

        if (node is null) 
            return new();

        var contract = FromNode(node);

        if (record.TryGetValue(Aliases.Vendor, out var vendorValue))
        {
            var vendorNode = vendorValue?.As<INode>();
            //Console.WriteLine($"FOUND Contract's Vendor: {vendorNode?.Id}");
            contract.Vendor = vendorNode is null ? null : Vendor.FromNode(vendorNode);
        }

        if (record.TryGetValue(Aliases.ItemWithQuantityList, out var itemListValue))
        {
            var itemListNode = itemListValue?.As<IList<IDictionary<string, object>>>();
            contract.Items = itemListNode is null ? null : ItemWithQuantity.ListFromList(itemListNode);
        }

        return contract;
    }

    public static Contract FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        DeliveryDate = node.Properties["deliveryDate"].As<LocalDate>()
    };
}
