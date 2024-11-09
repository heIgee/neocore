using Neocore.Models;

namespace Neocore.Repositories.Abstract;

public interface IItemRepository
{
    Task<IEnumerable<Item>> FindAll();
    Task<Item?> FindById(int id);
}