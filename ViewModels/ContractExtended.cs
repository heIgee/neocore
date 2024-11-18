using Neo4j.Driver;
using Neocore.Common;
using Neocore.ViewModels;

namespace Neocore.Models;

public class ContractExtended : INeocoreNode<ContractExtended>
{
    public int? Id { get; set; }
    public LocalDate? DeliveryDate { get; set; }

    public Vendor? Vendor { get; set; }
    public List<DeliveredItem>? Items { get; set; }

    public static ContractExtended FromRecord(IRecord record)
    {
        var node = record[Al.Contract].As<INode>();

        if (node is null) 
            return new();

        var contract = FromNode(node);

        if (record.TryGetValue(Al.Vendor, out var vendorValue))
        {
            var vendorNode = vendorValue?.As<INode>();
            contract.Vendor = vendorNode is null ? null : Vendor.FromNode(vendorNode);
        }

        if (record.TryGetValue(Al.DeliveredItemList, out var itemListValue))
        {
            var itemListNode = itemListValue?.As<IList<IDictionary<string, object>>>();
            contract.Items = itemListNode is null ? null : DeliveredItem.ListFromDictionaries(itemListNode);
        }

        return contract;
    }

    public static ContractExtended FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        DeliveryDate = node.Properties["deliveryDate"].As<LocalDate>()
    };
}
