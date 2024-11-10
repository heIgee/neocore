using Neo4j.Driver;

namespace Neocore.Models;

public class ItemWithQuantity
{
    public Item? Item { get; set; }
    public int? Quantity { get; set; }

    public static List<ItemWithQuantity>? ListFromDictionaries(IList<IDictionary<string, object>>? itemListNode)
    {
        if (itemListNode is null || !itemListNode.Any())
            return null;

        return itemListNode.Select(itemDict =>
        {
            var itemNode = itemDict["item"]?.As<INode>();
            var quantity = itemDict["quantity"]?.As<int>();
            return new ItemWithQuantity
            {
                Item = itemNode is null ? null : Item.FromNode(itemNode),
                Quantity = quantity
            };
        }).ToList();
    }
}
