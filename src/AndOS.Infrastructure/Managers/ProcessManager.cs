using AndOS.Application.Entities;
using AndOS.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AndOS.Infrastructure.Managers;

public class ProcessManager(ILogger<ProcessManager> logger) : IProcessManager
{
    public List<Process> Processes => [.. _processes];
    readonly ObservableCollection<Process> _processes = [];

    public async Task EndAsync(Process process)
    {
        try
        {
            if (process.Window is Window)
            {
                await destroyWindowAsync(process);
                process.Window = null;
            }

            _processes.Remove(process);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(EndAsync));
            throw;
        }
    }

    public async Task MinimizeToTray(Process process)
    {
        try
        {
            if (process.Window is Window)
                await destroyWindowAsync(process);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(MinimizeToTray));
            throw;
        }
    }

    public async Task RestoreFromTray(Process process)
    {
        try
        {
            if (process.Window is Window window)
                await instanceWindowAsync(process);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(RestoreFromTray));
            throw;
        }
    }

    public async Task<Process> StartAsync(Program program)
    {
        try
        {
            if (program.AllowMultipleInstances)
                return await instanceProcess(program);
            else
            {
                var process = Processes.Find(x => x.Program.GetType() == program.GetType());

                if (process is Process)
                {
                    if (process.Window is Window window)
                        await FocusWindowAsync(window);
                    else
                        await RestoreFromTray(process);
                }
                else
                    process ??= await instanceProcess(program);

                return process;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(StartAsync));
            throw;
        }
    }

    private async Task<Process> instanceProcess(Program program)
    {
        var process = new Process(null, program);
        _processes.Add(process);

        if (program.InstantiateWindowOnOpen)
            await instanceWindowAsync(process);

        return process;
    }

    private IEnumerable<Window> _windows => _processes.Where(x => x.Window is not null).Select(x => x.Window);
    public event Func<Window, Task> OnWindowOpen;
    public event Func<Window, Task> OnWindowClose;
    public event Func<Window, Task> OnWindowFocus;
    public event Func<Window, Task> OnWindowHide;

    async Task destroyWindowAsync(Process process)
    {
        var window = process.Window;
        process.Window = null;
        await OnWindowClose?.Invoke(window);
        await FocusOtherWindowAsync();
    }

    public async Task FocusOtherWindowAsync()
    {
        if (!_windows.Any(w => !w.Hide))
            return;
        var window = _windows.Where(w => !w.Hide).MaxBy(x => x.Index);

        window.Focused = true;
        if (OnWindowFocus is not null)
        {
            await OnWindowFocus?.Invoke(window);
        }
    }

    public async Task HideWindowAsync(Window window)
    {
        window.Focused = false;
        window.Hide = true;
        if (OnWindowHide is not null)
            await OnWindowHide.Invoke(window);
        await FocusOtherWindowAsync();
    }

    public async Task FocusWindowAsync(Window window)
    {
        internalFocusWindow(window);
        await OnWindowFocus?.Invoke(window);

        if (_windows.Count() == 1)
            return;

        Window oldWindow = _windows.Except([window]).Where(w => !w.Hide).MaxBy(x => x.Index);
        if (oldWindow is null)
            return;

        internalFocusWindow(oldWindow, window);

        if (OnWindowFocus is not null)
            await OnWindowFocus?.Invoke(oldWindow);
    }

    private void internalFocusWindow(Window window)
    {
        window.Focused = true;
        window.Hide = false;
    }

    private void internalFocusWindow(Window oldWindow, Window newWindow)
    {
        logger.Log(LogLevel.Debug, "Index old window {0} and new window {1}", oldWindow.Index, newWindow.Index);
        if (oldWindow.Index > newWindow.Index)
        {
            int tempIndex = oldWindow.Index;
            oldWindow.Index = newWindow.Index;
            newWindow.Index = tempIndex;
        }
        oldWindow.Focused = false;
        newWindow.Focused = true;
        newWindow.Hide = false;
    }

    async Task instanceWindowAsync(Process process)
    {
        int nextIndex = _windows.Count() > 0 ? _windows.Max(x => x.Index) + 1 : 1;
        Window newWindow = new() { Draggable = true, Resize = true, Title = $"Window {_windows.Count()}", Index = nextIndex, Focused = true };
        process.Window = newWindow;
        await OnWindowOpen?.Invoke(newWindow);
        await FocusWindowAsync(newWindow);
    }
}
