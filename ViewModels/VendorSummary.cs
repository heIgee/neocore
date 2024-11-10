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
        Vendor = Vendor.FromRecord(record),
        TotalContracts = record[Al.CountDistinctContracts].As<int>(),
        TotalItems = record[Al.CountDistinctItems].As<int>()
    };
}

// foreach (var c in Employee.Contracts) foreach (var i in c.Items) count++ 


