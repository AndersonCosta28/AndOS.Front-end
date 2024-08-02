using AndOS.Domain.Enums;

namespace AndOS.Application.Interfaces;

public interface IProgramManager
{
    event Func<Program, Task> OnInstall;
    event Func<Program, Task> OnUninstall;
    List<Program> Programs { get; }
    Task AddExternalProgramAsync(byte[] binary);
    Task AddExternalProgramAsync(Assembly assembly);
    Task AddExternalProgramAsync(string path);
    Task RemoveExternalAssemblyAsync(Assembly assembly);
    Task LoadAssembliesAsync();

    bool VerifyIfExistsAssembly(Assembly assembly);
    bool VerifyIfExistsAssembly(Assembly assembly, out Assembly assemblyExisting);
    bool IsValidModule(Assembly assembly);

    ResultCompareVersion CheckVersionAssembly(Assembly oldAssembly, Assembly newAssembly);
}