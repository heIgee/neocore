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
            .Return(@$"{Al.Vendor}, 
                COUNT(DISTINCT {Al.Contract}) AS {Al.CountDistinctContracts},
                COUNT(DISTINCT {Al.Item}) AS {Al.CountDistinctItems}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            VendorSummary.FromRecord
        );
    }

    //public async Task<IEnumerable<Employee>> FindByItemType(string productType)
    //{
    //    const string query = @$"
    //        MATCH ({Al.Employee}:Employee)
    //        <-[:SIGNED_WITH]-(ContractExtended)
    //        <-[:SUPPLIED_UNDER]-(p:Item {{type: $productType}}) 
    //        RETURN DISTINCT {Al.Employee}
    //    ";
    //    return await ExecuteReadListAsync(
    //        query,
    //        new { productType },
    //        Employee.FromRecord
    //    );
    //}
        
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

        //await Delete(id);
        //await Add(vendor);

        var query = new StringBuilder(@" 
            MATCH (v:Vendor {id: $id})
            SET v.name = $name, v.contactInfo = $contactInfo
        "); // TODO aliases

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

        var query = new StringBuilder(@" 
            CREATE (v:Vendor {id: $id, name: $name, contactInfo: $contactInfo})
        "); // TODO aliases

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
