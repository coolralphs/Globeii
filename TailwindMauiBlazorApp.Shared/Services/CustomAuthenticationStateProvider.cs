using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

//namespace TailwindMauiBlazorApp.Shared.Services;

//public class CustomAuthenticationStateProvider : AuthenticationStateProvider
//{
//    public override Task<AuthenticationState> GetAuthenticationStateAsync()
//    {
//        // Return anonymous user; replace this with real logic later
//        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
//        return Task.FromResult(new AuthenticationState(anonymous));
//    }
//}

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_currentUser));

    public void MarkUserAsLoggedOut()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public void MarkUserAsLoggedIn(ClaimsPrincipal user)
    {
        _currentUser = user;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }
}