using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AndOS.Infrastructure.Authentication;

internal static class AuthenticationServiceCollection
{
    internal static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddAuthorizationCore();
        services.AddScoped<CustomAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}