using AndOS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AndOS.Infrastructure.Api;

internal static class ApiServiceCollection
{
    internal static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IFolderService, FolderService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<AuthenticationService>();
        return services;
    }
}