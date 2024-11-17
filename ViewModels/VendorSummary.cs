using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;

namespace Neocore.ViewModels;

public class VendorSummary : INeocoreViewModel<VendorSummary>
{
    public Vendor Vendor { get; set; } = new();
    public int TotalContracts { get; set; }
    public int TotalDistinctItems { get; set; }
    public int TotalItemsQuantity { get; set; }
    public int ItemsRepairedQuantity { get; set; }

    public double Reliability => TotalItemsQuantity == 0 
        ? 100.0 
        : 100.0 * (TotalItemsQuantity - ItemsRepairedQuantity) / TotalItemsQuantity;

    public static VendorSummary FromRecord(IRecord record) => new()
    {
        Vendor = Vendor.FromRecord(record),
        TotalContracts = record[Al.CountDistinctContracts].As<int>(),
        TotalDistinctItems = record[Al.CountDistinctItems].As<int>(),
        TotalItemsQuantity = record[Al.TotalItemsQuantity].As<int>(),
        ItemsRepairedQuantity = record[Al.CountItemsRepaired].As<int>()
    };
}
