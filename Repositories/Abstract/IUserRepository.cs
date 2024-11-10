using Neocore.Models;

namespace Neocore.Repositories.Abstract;

public interface IUserRepository
{
    Task<IEnumerable<User>> FindAll();
    Task<User?> FindById(int id);
    Task<User?> FindByUsername(string username);
    Task<User?> ValidateUser(string username, string password);
    Task Update(int id, User user);
    Task Add(User user);
    Task Delete(int id);
}
