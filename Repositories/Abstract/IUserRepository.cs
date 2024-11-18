using Neocore.Models;

namespace Neocore.Repositories.Abstract;
public interface IUserRepository
{
    Task Add(User user);
    Task Delete(int id);
    Task<IEnumerable<User>> FindAll();
    Task<User?> FindById(int id);
    Task<User?> FindByUsername(string username);
    Task Update(int id, User user);
    Task<User?> ValidateUser(string username, string password);
}