using Neocore.Models;

namespace Neocore.Repositories;

public interface IItemRepository
{
    Task<Item> FindById(int id);
}