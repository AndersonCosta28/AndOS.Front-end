﻿using AndOS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AndOS.Infrastructure.Managers;

internal static class ManagerServiceCollection
{
    internal static IServiceCollection AddManagerServices(this IServiceCollection services)
    {
        services.AddSingleton<IProgramManager, ProgramManager>();
        services.AddSingleton<IContextMenuManager, ContextMenuManager>();
        services.AddSingleton<IWindowManager, WindowManager>();
        services.AddSingleton<IProcessManager, ProcessManager>();
        return services;
    }
}