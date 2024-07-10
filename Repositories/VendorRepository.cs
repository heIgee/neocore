using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories;

public class VendorRepository(IDriver driver) : NeocoreRepository(driver), IVendorRepository
{
    public async Task<Vendor> FindById(int id)
    {
        const string query = $"MATCH ({Aliases.Vendor}:Vendor {{id: $id}}) RETURN {Aliases.Vendor}";
        return await ExecuteReadSingleAsync(
            query, 
            new { id },
            Vendor.FromRecord
        );
    }

    public async Task<IEnumerable<Vendor>> FindAll()
    {
        const string query = $"MATCH ({Aliases.Vendor}:Vendor) RETURN {Aliases.Vendor}";
        return await ExecuteReadListAsync(
            query,
            new {},
            Vendor.FromRecord
        );
    }

    public async Task<IEnumerable<VendorSummary>> FindAllWithSummary()
    {
        const string query = @$"
            MATCH ({Aliases.Vendor}:Vendor)
            <-[:SIGNED_WITH]-(c:Contract)
            <-[su:SUPPLIED_UNDER]-(i:Item) 
            RETURN {Aliases.Vendor}, 
                COUNT(DISTINCT c) AS {Aliases.CountDistinctContracts}, 
                COUNT(DISTINCT i) AS {Aliases.CountDistinctItems}
        ";
        return await ExecuteReadListAsync(
            query,
            new {},
            VendorSummary.FromRecord
        );
    }

    public async Task<IEnumerable<Vendor>> FindByItemType(string productType)
    {
        const string query = @$"
            MATCH ({Aliases.Vendor}:Vendor)
            <-[:SIGNED_WITH]-(Contract)
            <-[:SUPPLIED_UNDER]-(p:Item {{type: $productType}}) 
            RETURN DISTINCT {Aliases.Vendor}
        ";
        return await ExecuteReadListAsync(
            query,
            new { productType },
            Vendor.FromRecord
        );
    }
}
