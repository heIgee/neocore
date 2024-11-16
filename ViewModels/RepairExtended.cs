using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;

namespace Neocore.ViewModels;

public class RepairExtended
{
    
    public Repair? Repair { get; set; }
    public Employee? Employee { get; set; }
    public Customer? Customer { get; set; }
    public Item? Item { get; set; }

    public static RepairExtended FromRecord(IRecord record)
    {
        var repairNode = record[Al.Repair].As<INode>();

        if (repairNode is null)
            return new();

        var repairX = new RepairExtended() { Repair = Repair.FromNode(repairNode) };

        if (record.TryGetValue(Al.Employee, out var employeeValue))
        {
            var employeeNode = employeeValue?.As<INode>();
            repairX.Employee = employeeNode is null ? null : Employee.FromNode(employeeNode);
        }

        if (record.TryGetValue(Al.Customer, out var customerValue))
        {
            var customerNode = customerValue?.As<INode>();
            repairX.Customer = customerNode is null ? null : Customer.FromNode(customerNode);
        }

        if (record.TryGetValue(Al.Item, out var itemValue))
        {
            var itemNode = itemValue?.As<INode>();
            repairX.Item = itemNode is null ? null : Item.FromNode(itemNode);
        }

        return repairX;
    }
}
