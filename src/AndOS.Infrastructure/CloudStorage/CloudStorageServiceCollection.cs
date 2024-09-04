using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components.Extensions;

namespace AndOS.Infrastructure.CloudStorage;

internal static class CloudStorageServiceCollection
{
    internal static IServiceCollection AddCloudStorageServices(this IServiceCollection services)
    {
        services.AddScoped<ICloudStorageServiceFactory, CloudStorageServiceFactory>();
        services.AddKeyedScoped<ICloudStorageService, AzureStorageService>(AndOS.Core.Enums.CloudStorage.Azure_BlobStorage.GetDescription());
        return services;
    }
}