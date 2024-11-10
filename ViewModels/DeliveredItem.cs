using Neo4j.Driver;
using Neocore.Models;

namespace Neocore.ViewModels;

public class DeliveredItem
{
    public Item? Item { get; set; }
    public int? Quantity { get; set; }

    public static List<DeliveredItem>? ListFromDictionaries(IList<IDictionary<string, object>>? itemListNode)
    {
        if (itemListNode is null || !itemListNode.Any())
            return null;

        return itemListNode.Select(itemDict =>
        {
            var itemNode = itemDict["item"]?.As<INode>();
            var quantity = itemDict["quantity"]?.As<int>();
            return new DeliveredItem
            {
                Item = itemNode is null ? null : Item.FromNode(itemNode),
                Quantity = quantity
            };
        }).ToList();
    }
}
