using Neo4j.Driver;
using Neocore.Models;

namespace Neocore.Repositories;

public class VendorRepository(IDriver driver)
{
    private readonly IDriver _driver = driver;

    public async Task<Vendor> FindById(int id)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (v:Vendor {id: $id}) RETURN v", new { id });
            return await cursor.SingleAsync(record => MapToVendor(record["v"].As<INode>()));
        });
        return result;
    }

    public async Task<IEnumerable<Vendor>> FindAll()
    {
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (v:Vendor) RETURN v");
            return await cursor.ToListAsync(record => MapToVendor(record["v"].As<INode>()));
        });
        return result;
    }

    public async Task<IEnumerable<Vendor>> FindVendorsByProductType(string productType)
    {
        await using var session = _driver.AsyncSession();
        var result = await session.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(@"
                MATCH (v:Vendor)-[:SUPPLIES]->(p:Product {type: $productType}) 
                RETURN DISTINCT v
            ", new { productType });
            return await cursor.ToListAsync(record => MapToVendor(record["v"].As<INode>()));
        });
        return result;


        #if false
        const string query = @"
            MATCH (v:Vendor)-[:SUPPLIES]->(p:Product {type: $productType})
            RETURN DISTINCT v";

        return await ExecuteReadListAsync(
            query,
            new { productType },
            record => MapToVendor(record["v"].As<INode>()));
        #endif
    }


    private static Vendor MapToVendor(INode node) => new()
    {
        Id = node.Properties["id"].As<int>(),
        Name = node.Properties["name"].As<string>(),
        ContactInfo = node.Properties["contactInfo"].As<string>()
    };
}
