using AndOS.Application.Entities;
using AndOS.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AndOS.Infrastructure.Managers;

public class ProcessManager(
    IWindowManager windowManager,
    ILogger<ProcessManager> logger) : IProcessManager
{
    public List<Process> Processes => [.. _processes];
    readonly ObservableCollection<Process> _processes = [];

    public Task EndAsync(Process process)
    {
        try
        {
            if (process.Window is Window window)
            {
                windowManager.DestroyAsync(window);
                process.Window = null;
            }

            _processes.Remove(process);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(EndAsync));
            throw;
        }

        return Task.CompletedTask;
    }

    public Task MinimizeToTray(Process process)
    {
        try
        {
            if (process.Window is Window window)
            {
                windowManager.DestroyAsync(window);
                process.Window = null;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(MinimizeToTray));
            throw;
        }

        return Task.CompletedTask;
    }

    public Task RestoreFromTray(Process process)
    {
        try
        {
            if (process.Window is Window window)
                windowManager.InstanceAsync(process.Program);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(RestoreFromTray));
            throw;
        }

        return Task.CompletedTask;
    }

    public async Task<Process> StartAsync(Program program)
    {
        try
        {
            if (program.AllowMultipleInstances)
                return await instanceProcess(windowManager, program);
            else
            {
                var process = Processes.Find(x => x.Program.GetType() == program.GetType());

                if (process is Process)
                {
                    if (process.Window is Window window)
                        await windowManager.FocusAsync(window);
                    else
                        await RestoreFromTray(process);
                }
                else
                    process ??= await instanceProcess(windowManager, program);

                return process;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(StartAsync));
            throw;
        }
    }

    private async Task<Process> instanceProcess(IWindowManager windowManager, Program program)
    {
        var window = await windowManager.InstanceAsync(program);

        var process = new Process(window, program);
        _processes.Add(process);
        return process;
    }
}
