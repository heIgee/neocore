using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;

namespace Neocore.ViewModels;

public class VendorSummary : INeocoreViewModel<VendorSummary>
{
    public Vendor Vendor { get; set; } = new();
    public int TotalContracts { get; set; }
    public int TotalItems { get; set; }

    public static VendorSummary FromRecord(IRecord record) => new()
    {
        Vendor = Vendor.FromNode(record[Aliases.Vendor].As<INode>()),
        TotalContracts = record[Aliases.CountDistinctContracts].As<int>(),
        TotalItems = record[Aliases.CountDistinctItems].As<int>()
    };
}
