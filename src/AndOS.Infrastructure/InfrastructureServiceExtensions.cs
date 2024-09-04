using AndOS.Infrastructure.Api;
using AndOS.Infrastructure.Authentication;
using AndOS.Infrastructure.CloudStorage;
using AndOS.Infrastructure.Managers;
using AndOS.Module.FileExplorer.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Polly;
using Polly.Retry;

namespace AndOS.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthenticationServices();
        services.AddManagerServices();
        services.AddCloudStorageServices();
        services.AddApiServices();
        var apiConfiguration = configuration.GetSection("ApiConfiguration");

        services.AddHttpClient("API", client => client.BaseAddress = new Uri(configuration["ApiConfiguration:BaseUrl"]))
                .AddResilienceHandler("default", b =>
                {
                    b.AddRetry(new RetryStrategyOptions<HttpResponseMessage>()
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromSeconds(2),
                    });
                });

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

        services.AddFluentUIComponents();

        // Adiciona Blazor Bootstrap
        services.AddBlazorBootstrap();

        // Adiciona serviços específicos
        services.AddScoped<IFileExplorerDialogs, FileExplorerDialogs>();
        return services;
    }
}