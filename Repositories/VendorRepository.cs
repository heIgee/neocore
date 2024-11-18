using Neo4j.Driver;
using Neocore.Common;
using Neocore.Models;
using Neocore.Repositories.Abstract;
using Neocore.ViewModels;
using System.Text;

namespace Neocore.Repositories;

public class VendorRepository(IDriver driver) : NeocoreRepository(driver), IVendorRepository
{
    public async Task<Vendor?> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Al.Vendor}:Vendor)")
            .Where($"{Al.Vendor}.id = $id", "id", id)
            .Return($"{Al.Vendor}")
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
            .Match($"({Al.Vendor}:Vendor)")
            .Return($"{Al.Vendor}")
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
            .Match($"({Al.Vendor}:Vendor)")
            .OptionalMatch($"({Al.Vendor})<-[:SIGNED_WITH]-({Al.Contract}:Contract)")
            .OptionalMatch($"({Al.Contract})<-[:SUPPLIED_UNDER]-({Al.Item}:Item)")
            .OptionalMatch($"({Al.Item})<-[:INVOLVES]-({Al.Repair}:Repair)")
            .Return(@$"{Al.Vendor}, 
                COUNT(DISTINCT {Al.Contract}) AS {Al.CountDistinctContracts},
                COUNT(DISTINCT {Al.Item}) AS {Al.CountDistinctItems},
                COUNT(DISTINCT {Al.Repair}) AS {Al.CountItemsRepaired}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            VendorSummary.FromRecord
        );
    }

    public async Task Delete(int id)
    {
        const string query = @"
            MATCH (v:Vendor {id: $id})
            DETACH DELETE v
        ";

        await ExecuteWriteSingleAsync(
            query,
            new { id }
        );
    }

    public async Task Update(int id, Vendor vendor)
    {
        _ = await FindById(id) ?? throw new InvalidOperationException(@$"
            Cannot update non-existent {nameof(Vendor)} (id: {id})
        ");

        var query = new StringBuilder(@$" 
            MATCH ({Al.Vendor}:Vendor {{id: $id}})
            SET {Al.Vendor}.name = $name, {Al.Vendor}.contactInfo = $contactInfo
        ");

        object parameters = new
        {
            id,
            name = vendor.Name ?? string.Empty,
            contactInfo = vendor.ContactInfo ?? string.Empty
        };

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    public async Task Add(Vendor vendor)
    {
        int id = await NewId();

        var query = new StringBuilder(@$" 
            CREATE ({Al.Vendor}:Vendor {{id: $id, name: $name, contactInfo: $contactInfo}})
        ");

        object parameters = new
        {
            id,
            name = vendor.Name ?? string.Empty,
            contactInfo = vendor.ContactInfo ?? string.Empty
        };

        await ExecuteWriteSingleAsync(
            query.ToString(),
            parameters
        );
    }

    private async Task<int> NewId()
    {
        const string query = @$"
            MATCH (v:Vendor)
            RETURN v.id as id
            ORDER BY v.id desc
            LIMIT 1
        ";

        return await ExecuteReadSingleAsync(
            query,
            new { },
            r => r["id"].As<int>()
        ) + 1;
    }
}
