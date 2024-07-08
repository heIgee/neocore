using Neo4j.Driver;
using Neocore.Models;

namespace Neocore.ViewModels;

public class VendorSummary : INeocoreViewModel<VendorSummary>
{
    public Vendor Vendor { get; set; } = new();
    public int TotalContracts { get; set; }
    public int TotalItems { get; set; }

    public static VendorSummary FromRecord(IRecord record) => new()
    {
        Vendor = Vendor.FromNode(record["v"].As<INode>()),
        TotalContracts = record["cdc"].As<int>(),
        TotalItems = record["cdi"].As<int>()
    };
}
