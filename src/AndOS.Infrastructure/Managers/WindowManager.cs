using AndOS.Application.Entities;
using AndOS.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AndOS.Infrastructure.Managers;

internal class WindowManager(ILogger<WindowManager> logger) : IWindowManager
{
    private readonly ILogger<WindowManager> _logger = logger;

    public List<Window> Windows => [.. _windows];

    private ObservableCollection<Window> _windows { get; } = [];
    public event Func<Window, Task> OnOpen;
    public event Func<Window, Task> OnClose;
    public event Func<Window, Task> OnFocus;
    public event Func<Window, Task> OnHide;

    public async Task DestroyAsync(Window window)
    {
        _windows.Remove(window);
        await OnClose?.Invoke(window);

        await FocusOtherAsync();
    }

    public async Task FocusOtherAsync()
    {
        if (!_windows.Any(w => !w.Hide))
            return;
        Window window = _windows.Where(w => !w.Hide).MaxBy(x => x.Index);

        window.Focused = true;
        if (OnFocus is not null)
        {
            await OnFocus?.Invoke(window);
        }
    }

    public async Task HideAsync(Window window)
    {
        window.Focused = false;
        window.Hide = true;
        if (OnHide is not null)
            await OnHide.Invoke(window);
        await FocusOtherAsync();
    }

    public async Task FocusAsync(Window window)
    {
        internalFocus(window);
        await OnFocus?.Invoke(window);

        if (_windows.Count == 1)
            return;

        Window oldWindow = _windows.Except([window]).Where(w => !w.Hide).MaxBy(x => x.Index);
        if (oldWindow is null)
            return;

        internalFocus(oldWindow, window);

        if (OnFocus is not null)
            await OnFocus?.Invoke(oldWindow);
    }

    private void internalFocus(Window window)
    {
        window.Focused = true;
        window.Hide = false;
    }

    private void internalFocus(Window oldWindow, Window newWindow)
    {
        _logger.LogInformation("Index old window {0} and new window {1}", oldWindow.Index, newWindow.Index);
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

    public async Task<Window> InstanceAsync(Program program)
    {
        int nextIndex = _windows.Count > 0 ? _windows.Max(x => x.Index) + 1 : 1;
        Window newWindow = new() { Draggable = true, Resize = true, Title = $"Window {_windows.Count}", Index = nextIndex, Focused = true };
        _windows.Add(newWindow);
        await OnOpen?.Invoke(newWindow);
        await FocusAsync(newWindow);

        return newWindow;
    }
}