using Neo4j.Driver;
using Neocore.Common;
using Neocore.Components.Pages;
using Neocore.Filters;
using Neocore.Models;
using System.Text;

namespace Neocore.Repositories;

public class ContractRepository(IDriver driver) : NeocoreRepository(driver), IContractRepository
{
    public async Task<Contract> FindById(int id)
    {
        const string query = $"MATCH ({Aliases.Contract}:Contract {{id: $id}}) RETURN {Aliases.Contract}";
        return await ExecuteReadSingleAsync(
            query,
            new { id },
            Contract.FromRecord
        );
    }

    public async Task<IEnumerable<Contract>> FindAll()
    {
        const string query = $"MATCH ({Aliases.Contract}:Contract) RETURN {Aliases.Contract}";
        return await ExecuteReadListAsync(
            query,
            new { },
            Contract.FromRecord
        );
    }

    public async Task<IEnumerable<Contract>> FindByVendor(int vendorId)
    {
        const string query = @$"
            MATCH ({Aliases.Vendor}:Vendor {{id: $vendorId}})<-[:SIGNED_WITH]-({Aliases.Contract}:Contract)
            RETURN DISTINCT {Aliases.Contract}
        ";
        return await ExecuteReadListAsync(
            query,
            new { vendorId },
            Contract.FromRecord
        );
    }

    public async Task<IEnumerable<Contract>> FindByFilter(ContractFilter filter)
    {
        var query = new StringBuilder($"MATCH ({Aliases.Contract}:Contract)");
        var parameters = new Dictionary<string, object>();

        if (filter.VendorId.HasValue)
        {
            query.Append($" -[:SIGNED_WITH]-> ({Aliases.Vendor}:Vendor {{id: $vendorId}})");
            parameters["vendorId"] = filter.VendorId.Value;
        }

        if (filter.DeliveryDateFrom.HasValue)
        {
            query.Append($" WHERE {Aliases.Contract}.deliveryDate >= $deliveryDateFrom");
            parameters["deliveryDateFrom"] = filter.DeliveryDateFrom.Value;
        }

        query.Append($" RETURN DISTINCT {Aliases.Contract}");

        return await ExecuteReadListAsync(
            query.ToString(),
            parameters,
            Contract.FromRecord
        );
    }
}
