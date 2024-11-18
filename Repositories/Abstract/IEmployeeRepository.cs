using Neocore.Models;

namespace Neocore.Repositories.Abstract;
public interface IEmployeeRepository
{
    Task Add(Employee employee);
    Task Delete(int id);
    Task<IEnumerable<Employee>> FindAll();
    Task<Employee?> FindById(int id);
    Task Update(int id, Employee employee);
}