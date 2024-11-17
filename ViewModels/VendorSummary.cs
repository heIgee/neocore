using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;

namespace Neocore.ViewModels;

public class VendorSummary : INeocoreViewModel<VendorSummary>
{
    public Vendor Vendor { get; set; } = new();
    public int TotalContracts { get; set; }
    public int TotalItems { get; set; }
    public int ItemsRepaired { get; set; }

    public double Reliability => TotalItems == 0 
        ? 100.0 
        : 100.0 * (TotalItems - ItemsRepaired) / TotalItems;

    public static VendorSummary FromRecord(IRecord record) => new()
    {
        Vendor = Vendor.FromRecord(record),
        TotalContracts = record[Al.CountDistinctContracts].As<int>(),
        TotalItems = record[Al.CountDistinctItems].As<int>(),
        ItemsRepaired = record[Al.CountItemsRepaired].As<int>()
    };
}
