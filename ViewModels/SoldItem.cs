using Neo4j.Driver;
using Neocore.Models;

namespace Neocore.ViewModels;

public class SoldItem
{
    public Item? Item { get; set; }
    public int? Quantity { get; set; }
    public string? WarrantyTerms { get; set; }

    public static List<SoldItem>? ListFromDictionaries(IList<IDictionary<string, object>>? itemListNode)
    {
        if (itemListNode is null || !itemListNode.Any())
            return null;

        return itemListNode.Select(itemDict =>
        {
            var itemNode = itemDict["item"]?.As<INode>();
            var quantity = itemDict["quantity"]?.As<int>();
            var warrantyTerms = itemDict["warrantyTerms"].As<string>();
            return new SoldItem
            {
                Item = itemNode is null ? null : Item.FromNode(itemNode),
                Quantity = quantity,
                WarrantyTerms = warrantyTerms
            };
        }).ToList();
    }
}
