using Neocore.Models;

namespace Neocore.Repositories;

public interface IProductRepository
{
    Task<Product> FindById(int id);
}