using Blazored.LocalStorage;
using Neocore.Models;
using Neocore.Repositories.Abstract;

namespace Neocore.Auth;

public class AuthService(IUserRepository userRepository, ILocalStorageService localStorage) : IAuthService
{
    public User? CurrentUser { get; private set; }

    public async Task InitializeUser()
    {
        var userId = await localStorage.GetItemAsync<int?>("userId");
        //Console.WriteLine(userId);
        if (userId.HasValue)
        {
            CurrentUser = await userRepository.FindById(userId.Value);
        }
        //Console.WriteLine(CurrentUser);
        //Console.WriteLine(((IAuthService)this).IsAuthenticated);
    }

    public async Task<bool> SignIn(string username, string password)
    {
        var user = await userRepository.ValidateUser(username, password);
        if (user == null) return false;
        
        CurrentUser = user;
        await localStorage.SetItemAsync("userId", user.Id);

        return true;
    }

    public async Task SignOut()
    {
        CurrentUser = null;
        await localStorage.RemoveItemAsync("userId");
    }

    public bool HasAccess(UserRole minimumRole) 
    {
        return (CurrentUser?.Role ?? UserRole.Viewer) >= minimumRole;
    }
}
