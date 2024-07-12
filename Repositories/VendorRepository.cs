using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories;

public class VendorRepository(IDriver driver) : NeocoreRepository(driver), IVendorRepository
{
    public async Task<Vendor> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Aliases.Vendor}:Vendor)")
            .Where($"{Aliases.Vendor}.id = $id", "id", id)
            .Return($"{Aliases.Vendor}")
            .Build();

        return await ExecuteReadSingleAsync(
            query, 
            parameters,
            Vendor.FromRecord
        );
    }

    public async Task<IEnumerable<Vendor>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Aliases.Vendor}:Vendor)")
            .Return($"{Aliases.Vendor}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            Vendor.FromRecord
        );
    }

    public async Task<IEnumerable<VendorSummary>> FindAllWithSummary()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Aliases.Vendor}:Vendor)")
            .OptionalMatch($"({Aliases.Vendor})<-[:SIGNED_WITH]-({Aliases.Contract}:Contract)")
            .OptionalMatch($"({Aliases.Contract})<-[:SUPPLIED_UNDER]-({Aliases.Item}:Item)")
            .Return(@$"{Aliases.Vendor}, 
                COUNT(DISTINCT {Aliases.Contract}) AS {Aliases.CountDistinctContracts},
                COUNT(DISTINCT {Aliases.Item}) AS {Aliases.CountDistinctItems}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            VendorSummary.FromRecord
        );
    }

    //public async Task<IEnumerable<Vendor>> FindByItemType(string productType)
    //{
    //    const string query = @$"
    //        MATCH ({Aliases.Vendor}:Vendor)
    //        <-[:SIGNED_WITH]-(Contract)
    //        <-[:SUPPLIED_UNDER]-(p:Item {{type: $productType}}) 
    //        RETURN DISTINCT {Aliases.Vendor}
    //    ";
    //    return await ExecuteReadListAsync(
    //        query,
    //        new { productType },
    //        Vendor.FromRecord
    //    );
    //}
}
