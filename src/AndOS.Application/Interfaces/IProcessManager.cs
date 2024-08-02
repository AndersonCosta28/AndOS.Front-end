namespace AndOS.Application.Interfaces;

public interface IProcessManager
{
    public event Func<Window, Task> OnWindowOpen;
    public event Func<Window, Task> OnWindowClose;
    public event Func<Window, Task> OnWindowFocus;
    public event Func<Window, Task> OnWindowHide;
    public List<Process> Processes { get; }

    Task<Process> StartAsync(Program program);
    Task EndAsync(Process process);
    Task MinimizeToTray(Process process);
    Task RestoreFromTray(Process process);

    Task FocusWindowAsync(Window window);
    Task FocusOtherWindowAsync();
    Task HideWindowAsync(Window window);
}