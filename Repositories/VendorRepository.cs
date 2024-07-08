using Neo4j.Driver;
using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories;

public class VendorRepository(IDriver driver) : NeocoreRepository(driver), IVendorRepository
{
    public async Task<Vendor> FindById(int id)
    {
        const string query = "MATCH (v:Vendor {id: $id}) RETURN v";
        return await ExecuteReadSingleAsync<Vendor>(
            query, 
            new { id },
            record => Vendor.FromNode(record["v"].As<INode>())
        );
    }

    public async Task<IEnumerable<Vendor>> FindAll()
    {
        const string query = "MATCH (v:Vendor) RETURN v";
        return await ExecuteReadListAsync(
            query,
            new {},
            record => Vendor.FromNode(record["v"].As<INode>())
        );
    }

    public async Task<IEnumerable<VendorSummary>> FindAllWithSummary()
    {
        const string query = @"
            MATCH (v:Vendor)<-[:SIGNED_WITH]-(c:Contract)<-[su:SUPPLIED_UNDER]-(i:Item) 
            RETURN v, COUNT(DISTINCT c) AS cdc, COUNT(DISTINCT i) AS cdi
        ";
        return await ExecuteReadListAsync(
            query,
            new {},
            VendorSummary.FromRecord
        );
    }

    public async Task<IEnumerable<Vendor>> FindVendorsByItemType(string productType)
    {
        const string query = @"
            MATCH (v:Vendor)<-[:SIGNED_WITH]-(Contract)<-[:SUPPLIED_UNDER]-(p:Item {type: $productType}) 
            RETURN DISTINCT v
        ";
        return await ExecuteReadListAsync(
            query,
            new { productType },
            record => Vendor.FromNode(record["v"].As<INode>())
        );
    }
}
