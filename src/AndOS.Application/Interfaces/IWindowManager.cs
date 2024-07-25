using AndOS.Application.Entities;

namespace AndOS.Application.Interfaces;

public interface IWindowManager
{
    public List<Window> Windows { get; }

    public event Func<Window, Task> OnOpen;
    public event Func<Window, Task> OnClose;
    public event Func<Window, Task> OnFocus;
    public event Func<Window, Task> OnHide;

    Task<Window> InstanceAsync(Program program);

    Task DestroyAsync(Window window);

    Task FocusAsync(Window window);

    Task FocusOtherAsync();

    Task HideAsync(Window window);
}