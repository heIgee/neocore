using Neocore.Models;

namespace Neocore.Auth;

public interface IAuthService
{
    User? CurrentUser { get; }
    bool IsAuthenticated => CurrentUser != null;
    Task InitializeUser();
    Task<bool> SignIn(string username, string password);
    Task SignOut();
    bool HasAccess(UserRole minimumRole);
}
