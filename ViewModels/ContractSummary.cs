using Neo4j.Driver;
using Neocore.Models;

namespace Neocore.ViewModels;

// TODO: deprecated?
public class ContractSummary : INeocoreViewModel<ContractSummary>
{
    public Contract? Contract { get; set; } = new();
    public Vendor? Vendor { get; set; } = new();

    public static ContractSummary FromRecord(IRecord record) => new()
    {
        Contract = Contract.FromRecord(record),
        Vendor = Vendor.FromRecord(record),
    };
}
