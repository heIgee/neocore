using Neocore.Models;

namespace Neocore.Repositories.Abstract;

public interface IUserRepository
{
    Task<User?> FindById(int id);
    Task<User?> ValidateUser(string username, string password);
}
