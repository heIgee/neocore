using Neocore.Models;

namespace Neocore.Repositories.Abstract;
public interface ICustomerRepository
{
    Task Add(Customer customer);
    Task Delete(int id);
    Task<IEnumerable<Customer>> FindAll();
    Task<Customer?> FindById(int id);
    Task<IEnumerable<Customer>> FindByVendor(int vendorId);
    Task Update(int id, Customer customer);
}