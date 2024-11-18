using Neocore.Filters;
using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories.Abstract;
public interface IContractRepository
{
    Task Add(ContractExtended contract, int? oldId = null);
    Task Delete(int id);
    Task<IEnumerable<ContractExtended>> FindAll();
    Task<IEnumerable<ContractExtended>> FindByFilter(ContractFilter filter);
    Task<IEnumerable<ContractSummary>> FindByFilterWithSummary(ContractFilter filter);
    Task<ContractExtended?> FindById(int id);
    Task Update(int id, ContractExtended contract);
}