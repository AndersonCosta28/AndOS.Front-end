using AndOS.Application.Entities;
using System.Collections.ObjectModel;

namespace AndOS.Application.Interfaces;

public interface IProcessManager
{
    public List<Process> Processes { get; }

    Task<Process> StartAsync(Program program);
    Task EndAsync(Process process);

    Task MinimizeToTray(Process process);
    Task RestoreFromTray(Process process);
}