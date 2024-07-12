using Neo4j.Driver;
using Neocore.Common;
using Neocore.Components.Pages;
using Neocore.Filters;
using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories;

public class ContractRepository(IDriver driver) : NeocoreRepository(driver), IContractRepository
{
    public async Task<Contract> FindById(int id)
    {
        var (query, parameters) = new QueryBuilder()
            .Match($"({Aliases.Contract}:Contract)")
            .Where($"{Aliases.Contract}.id = $id", "id", id)
            .Return($"{Aliases.Contract}")
            .Build();

        var contract = await ExecuteReadSingleAsync(
            query,
            parameters,
            Contract.FromRecord
        );

        return contract;
    }

    public async Task<IEnumerable<Contract>> FindAll()
    {
        var (query, _) = new QueryBuilder()
            .Match($"({Aliases.Contract}:Contract)")
            .Return($"{Aliases.Contract}")
            .Build();

        return await ExecuteReadListAsync(
            query,
            new { },
            Contract.FromRecord
        );
    }

    public async Task<IEnumerable<Contract>> FindByFilter(ContractFilter filter)
    {
        var builder = new QueryBuilder()
            .Match($"({Aliases.Contract}:Contract)-[:SIGNED_WITH]->({Aliases.Vendor}:Vendor)");
            //.OptionalMatch($"({Aliases.Contract}) -[:SIGNED_WITH]-> ({Aliases.Vendor}:Vendor)");

        filter.Apply(builder);

        builder.Return($" DISTINCT {Aliases.Contract}, {Aliases.Vendor}");

        var (query, parameters) = builder.Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            Contract.FromRecord
        );
    }

    public async Task<IEnumerable<ContractSummary>> FindByFilterWithSummary(ContractFilter filter)
    {
        var builder = new QueryBuilder()
            //.Match($"({Aliases.Contract}:Contract)")
            //.OptionalMatch($"({Aliases.Contract})-[:SIGNED_WITH]->({Aliases.Vendor}:Vendor)");
            .Match($"({Aliases.Contract}:Contract)-[:SIGNED_WITH]->({Aliases.Vendor}:Vendor)");
            //.OptionalMatch($"({Aliases.Contract})<-[r:SUPPLIED_UNDER]-({Aliases.Item}:Item)");
        filter.Apply(builder);

        builder.Return($"DISTINCT {Aliases.Contract}, {Aliases.Vendor}");

        var (query, parameters) = builder.Build();

        return await ExecuteReadListAsync(
            query,
            parameters,
            ContractSummary.FromRecord
        );
    }   

    //private static void ApplyFilter(QueryBuilder builder, ContractFilter filter)
    //{
    //    if (filter.VendorId.HasValue)
    //    {
    //        builder.Where($"{Aliases.Vendor}.id = $vendorId", "vendorId", filter.VendorId.Value);
    //    }

    //    if (filter.DeliveryDateFrom.HasValue)
    //    {
    //        builder.Where($"{Aliases.Contract}.deliveryDate >= $deliveryDateFrom", "deliveryDateFrom", filter.DeliveryDateFrom.Value);
    //    }
    //}
}
