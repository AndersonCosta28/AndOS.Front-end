using AndOS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AndOS.Infrastructure.Managers;

internal static class ManagerServiceCollection
{
    internal static IServiceCollection AddManagerServices(this IServiceCollection services)
    {
        services.AddSingleton<IProgramManager, ProgramManager>();
        services.AddSingleton<IContextMenuManager, ContextMenuManager>();
        services.AddSingleton<IProcessManager, ProcessManager>();
        services.AddSingleton<IAssemblyManager, IndexDbAssemblyManager>();
        return services;
    }
}