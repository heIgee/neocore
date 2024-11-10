using Neo4j.Driver;
using Neocore.Common;

namespace Neocore.Models;

// TODO ContractExtended
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
            contract.Vendor = vendorNode is null ? null : Vendor.FromNode(vendorNode);
        }

        if (record.TryGetValue(Aliases.ItemWithQuantityList, out var itemListValue))
        {
            var itemListNode = itemListValue?.As<IList<IDictionary<string, object>>>();
            contract.Items = itemListNode is null ? null : ItemWithQuantity.ListFromDictionaries(itemListNode);
        }

        return contract;
    }

    public static Contract FromNode(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        DeliveryDate = node.Properties["deliveryDate"].As<LocalDate>()
    };

    //public static Contract FromNode(INode node)
    //{
    //    var contract = new Contract();
    //    var props = node.Properties;

    //    contract.Id = props.TryGetValue("id", out var id)
    //        ? id.As<int>() : null;

    //    contract.DeliveryDate = props.TryGetValue("deliveryDate", out var deliveryDate)
    //        ? deliveryDate.As<LocalDate>() : null;

    //    return contract;
    //}
}
