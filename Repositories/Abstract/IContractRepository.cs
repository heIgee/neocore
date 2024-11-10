using Neocore.Filters;
using Neocore.Models;

namespace Neocore.Repositories.Abstract;
public interface IContractRepository
{
    Task<IEnumerable<ContractExtended>> FindAll();
    Task<IEnumerable<ContractExtended>> FindByFilter(ContractFilter filter);
    Task<ContractExtended?> FindById(int id);
}