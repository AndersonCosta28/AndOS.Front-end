using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AndOS.Infrastructure.Managers;

public class ProcessManager(ILogger<ProcessManager> logger) : IProcessManager
{
    #region Process
    public List<Process> Processes => [.. _processes];
    readonly ObservableCollection<Process> _processes = [];
    public event Func<Program, Dictionary<string, object>, Task> OnUpdateProgramArgumentsAsync;

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
            await instanceWindowAsync(process);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(RestoreFromTray));
            throw;
        }
    }

    public async Task StartAsync(Program program, Dictionary<string, object> arguments = default)
    {
        try
        {
            if (program.AllowMultipleInstances)
            {
                await instanceProcess(program, arguments);
                return;
            }
            else
            {
                var type = program.GetType();
                var process = Processes.Find(x => x.Program.GetType() == type);

                if (process is Process)
                {
                    if (process.Window is Window window)
                        await FocusWindowAsync(window);
                    else
                        await RestoreFromTray(process);

                    if (OnUpdateProgramArgumentsAsync != null)
                        await OnUpdateProgramArgumentsAsync.Invoke(process.Program, arguments);
                }
                else
                    process = await instanceProcess(program, arguments);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error from {0}.{1}", nameof(ProcessManager), nameof(StartAsync));
            throw;
        }
    }

    private async Task<Process> instanceProcess(Program program, Dictionary<string, object> arguments)
    {
        var newInstance = (Program)Activator.CreateInstance(program.GetType());

        var process = new Process(newInstance);
        _processes.Add(process);

        if (newInstance.InstantiateWindowOnOpen)
            await instanceWindowAsync(process);

        if (OnUpdateProgramArgumentsAsync != null)
            await OnUpdateProgramArgumentsAsync.Invoke(newInstance, arguments);

        return process;
    }
    #endregion

    #region Window
    private IEnumerable<Window> _windows => _processes.Where(x => x.Window is not null).Select(x => x.Window);
    public event Func<Window, Task> OnWindowOpenAsync;
    public event Func<Window, Task> OnWindowCloseAsync;
    public event Func<Window, Task> OnWindowFocusAsync;
    public event Func<Window, Task> OnWindowHideAsync;

    async Task destroyWindowAsync(Process process)
    {
        var window = process.Window;
        process.Window = null;
        await OnWindowCloseAsync?.Invoke(window);
        await FocusOtherWindowAsync();
    }

    public async Task FocusOtherWindowAsync()
    {
        if (!_windows.Any(w => !w.Hide))
            return;
        var window = _windows.Where(w => !w.Hide).MaxBy(x => x.Index);

        window.Focused = true;
        if (OnWindowFocusAsync is not null)
        {
            await OnWindowFocusAsync?.Invoke(window);
        }
    }

    public async Task HideWindowAsync(Window window)
    {
        window.Focused = false;
        window.Hide = true;
        if (OnWindowHideAsync is not null)
            await OnWindowHideAsync.Invoke(window);
        await FocusOtherWindowAsync();
    }

    public async Task FocusWindowAsync(Window window)
    {
        internalFocusWindow(window);
        await OnWindowFocusAsync?.Invoke(window);

        if (_windows.Count() == 1)
            return;

        var oldWindow = _windows.Except([window]).Where(w => !w.Hide).MaxBy(x => x.Index);
        if (oldWindow is null)
            return;

        internalFocusWindow(oldWindow, window);

        if (OnWindowFocusAsync is not null)
            await OnWindowFocusAsync?.Invoke(oldWindow);
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
        var nextIndex = this._windows.Any() ? this._windows.Max(x => x.Index) + 1 : 1;
        var newWindow = new Window() { Draggable = true, Resize = true, Title = $"Window {_windows.Count()}", Index = nextIndex, Focused = true };
        process.Window = newWindow;
        await OnWindowOpenAsync?.Invoke(newWindow);
        await this.FocusWindowAsync(newWindow);
    }
    #endregion
}