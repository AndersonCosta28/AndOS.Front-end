using AndOS.Application.Interfaces;
using System.Security.Claims;

namespace AndOS.Infrastructure.Authentication;

public class UserInfo(CustomAuthenticationStateProvider authenticationStateProvider) : IUserInfo
{
    public async Task<string> GetUserNameAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        var claim = user.FindFirst(ClaimTypes.Name) ?? user.FindFirst("name");
        var value = claim?.Value ?? "  ";
        return value;
    }

    public async Task<string> GetEmailAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        var claim = user.FindFirst(ClaimTypes.Email) ?? user.FindFirst("email");
        var value = claim?.Value ?? "  ";
        return value;
    }
}
