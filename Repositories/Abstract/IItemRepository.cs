using Neocore.Filters;
using Neocore.Models;

namespace Neocore.Repositories.Abstract;
public interface IItemRepository
{
    Task Add(Item item);
    Task Delete(int id);
    Task<IEnumerable<Item>> FindAll();
    Task<IEnumerable<Item>> FindByFilter(ItemFilter filter);
    Task<Item?> FindById(int id);
    Task<IEnumerable<Item>> FindByVendor(int vendorId);
    Task Update(int id, Item item);
}