namespace AndOS.Application.Interfaces;

public interface IProcessManager
{
    public event Func<Window, Task> OnWindowOpenAsync;
    public event Func<Window, Task> OnWindowCloseAsync;
    public event Func<Window, Task> OnWindowFocusAsync;
    public event Func<Window, Task> OnWindowHideAsync;
    public event Func<Program, Dictionary<string, object>, Task> OnUpdateProgramArgumentsAsync;

    public List<Process> Processes { get; }

    Task StartAsync(Program program, Dictionary<string, object> arguments = default);
    Task EndAsync(Process process);
    Task MinimizeToTray(Process process);
    Task RestoreFromTray(Process process);

    Task FocusWindowAsync(Window window);
    Task FocusOtherWindowAsync();
    Task HideWindowAsync(Window window);
}