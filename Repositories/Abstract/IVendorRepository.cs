using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories.Abstract;
public interface IVendorRepository
{
    Task Add(Vendor vendor);
    Task Delete(int id);
    Task<IEnumerable<Vendor>> FindAll();
    Task<IEnumerable<VendorSummary>> FindAllWithSummary();
    Task<Vendor?> FindById(int id);
    Task Update(int id, Vendor vendor);
}