using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;

namespace Neocore.ViewModels;

public class SaleExtended
{
    public Sale? Sale { get; set; }
    public Employee? Employee { get; set; }
    public Customer? Customer { get; set; }
    public List<SoldItem>? Items { get; set; }

    public static SaleExtended FromRecord(IRecord record)
    {
        var saleNode = record[Al.Sale].As<INode>();

        if (saleNode is null)
            return new();

        var saleX = new SaleExtended() { Sale = Sale.FromNode(saleNode) };

        if (record.TryGetValue(Al.Employee, out var employeeValue))
        {
            var employeeNode = employeeValue?.As<INode>();
            saleX.Employee = employeeNode is null ? null : Employee.FromNode(employeeNode);
        }

        if (record.TryGetValue(Al.Customer, out var customerValue))
        {
            var customerNode = customerValue?.As<INode>();
            saleX.Customer = customerNode is null ? null : Customer.FromNode(customerNode);
        }

        if (record.TryGetValue(Al.SoldItemList, out var itemListValue))
        {
            var itemListNode = itemListValue?.As<IList<IDictionary<string, object>>>();
            saleX.Items = itemListNode is null ? null : SoldItem.ListFromDictionaries(itemListNode);
        }

        return saleX;
    }
}
