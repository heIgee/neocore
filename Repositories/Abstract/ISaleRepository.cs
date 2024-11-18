using Neocore.Filters;
using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories.Abstract;
public interface ISaleRepository
{
    Task Add(SaleExtended saleX, int? oldId = null);
    Task Delete(int id);
    Task<IEnumerable<Sale>> FindAll();
    Task<Sale?> FindById(int id);
    Task<IEnumerable<SaleExtended>> FindExtendedByFilter(SaleFilter filter);
    Task<SaleExtended?> FindExtendedById(int id);
    Task Update(int id, SaleExtended saleX);
}