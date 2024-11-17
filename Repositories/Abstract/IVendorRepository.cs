using Neocore.Models;
using Neocore.ViewModels;

namespace Neocore.Repositories.Abstract;

public interface IVendorRepository
{
    Task<IEnumerable<Vendor>> FindAll();
    //Task<IEnumerable<VendorSummary>> FindAllWithSummary(int? itemId = null);
    Task<Vendor?> FindById(int id);
}