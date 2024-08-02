using AndOS.Application.Entities;
using AndOS.Application.Interfaces;
using AndOS.Domain.Consts;
using AndOS.Domain.Enums;
using AndOS.Module.FileExplorer;
using AndOS.Module.Notepad;
using AndOS.Module.UserConfiguration;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Reflection;

namespace AndOS.Infrastructure.Managers;

internal class ProgramManager : IProgramManager
{
    private readonly ILogger<ProgramManager> _logger;
    private readonly IAssemblyManager _assemblyManager;

    public ProgramManager(ILogger<ProgramManager> logger, IAssemblyManager assemblyManager)
    {
        _logger = logger;
        this._assemblyManager = assemblyManager;
        _programs.Add(new FileExplorerComponent());
        _programs.Add(new NotepadComponent());
        _programs.Add(new UserConfigurationComponent());
    }

    private readonly List<Assembly> _assemblies = [];
    public List<Program> Programs => _programs.ToList();
    ObservableCollection<Program> _programs { get; } = [];
    public event Func<Program, Task> OnInstall;
    public event Func<Program, Task> OnUninstall;

    public async Task LoadAssembliesAsync()
    {
        try
        {
            var assemblies = await _assemblyManager.GetAll();
            foreach (var assemblyInfo in assemblies)
            {
                var assembly = Assembly.Load(assemblyInfo.Binary);
                await LoadProgramFromAssembly(assembly);
                _logger.Log(LogLevel.Debug, $"Assembly loaded: {assemblyInfo.Name}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading assemblies from IndexedDB");
        }
    }

    public async Task RemoveExternalAssemblyAsync(Assembly assembly)
    {
        foreach (var program in _programs.Where(x => x is Program && x.Assembly == assembly)
            .ToList())
        {
            _programs.Remove(program);
            await OnUninstall?.Invoke(program);
        }
        _assemblies.Remove(assembly);
        await _assemblyManager.Remove(assembly);
    }

    public bool VerifyIfExistsAssembly(Assembly assembly)
    {
        var assemblyExisting = _assemblies.Find(x => x.GetName().Name == assembly.GetName().Name);
        return assemblyExisting != null;
    }

    public bool VerifyIfExistsAssembly(Assembly assembly, out Assembly assemblyExisting)
    {
        assemblyExisting = _assemblies.Find(x => x.GetName().Name == assembly.GetName().Name);
        return assemblyExisting != null;
    }

    public ResultCompareVersion CheckVersionAssembly(Assembly oldAssembly, Assembly newAssembly)
    {
        var versionOldAssembly = oldAssembly.GetName().Version;
        var versionNewAssembly = newAssembly.GetName().Version;
        if (versionOldAssembly == versionNewAssembly)
            return ResultCompareVersion.Equal;
        else if (versionOldAssembly > versionNewAssembly)
            return ResultCompareVersion.Lower;
        else
            return ResultCompareVersion.Higher;
    }

    public bool IsValidModule(Assembly assembly)
    {
        var attributesAssembly = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToList();

        var isValidModule = attributesAssembly.Find(a => a.Key.Equals(AssemblyConsts.TagModule))?.Value;
        _logger.Log(LogLevel.Debug, isValidModule?.ToString());
        return isValidModule == "true";
    }

    async Task LoadProgramFromAssembly(Assembly assembly)
    {
        List<Program> tempPrograms = [];
        try
        {
            // Obter todos os tipos no assembly atual
            var types = assembly.GetTypes();
            // Filtrar tipos que herdam de Program
            foreach (var type in types.Where(t => t.IsAssignableTo(typeof(Program)) && !t.IsAbstract))
            {
                _logger.Log(LogLevel.Debug, "Name type to instance {0}", type.FullName);
                // Instanciar o tipo
                Program programInstance = (Program)Activator.CreateInstance(type);
                //programInstance.SetAssembly(assembly);
                // Adicionar a instância à coleção
                tempPrograms.Add(programInstance);
            }
            foreach (var program in tempPrograms)
            {
                program.IsExternalProgram = true;
                _programs.Add(program);
                await OnInstall?.Invoke(program);
            }
            _assemblies.Add(assembly);
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Tratar exceções de tipo de carga
            Console.WriteLine($"Error on load program from assembly {assembly.FullName}: {ex.Message}");
        }
        finally
        {
            _logger.Log(LogLevel.Debug, "{0} programs loaders from assembly: {1}", _programs.Count, assembly.FullName);
        }
    }

    public async Task AddExternalProgramAsync(byte[] assemblybinary)
    {
        var assembly = Assembly.Load(assemblybinary);
        await LoadProgramFromAssembly(assembly);
        await _assemblyManager.Add(assembly, assemblybinary);
    }

    public async Task AddExternalProgramAsync(Assembly assembly)
    {

        var binaryData = File.ReadAllBytes(assembly.Location);
        await AddExternalProgramAsync(binaryData);
    }

    public async Task AddExternalProgramAsync(string path)
    {
        FileInfo file = new(path);
        var binaryData = File.ReadAllBytes(file.FullName);
        await AddExternalProgramAsync(binaryData);
    }
}