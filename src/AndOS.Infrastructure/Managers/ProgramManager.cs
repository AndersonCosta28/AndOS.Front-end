using AndOS.Module.Notepad;
using AndOS.Domain.Consts;
using AndOS.Module.FileExplorer;
using AndOS.Module.UserConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Reflection;
using AndOS.Domain.Enums;
using System.Collections.ObjectModel;
using AndOS.Application.Interfaces;
using AndOS.Application.Entities;

namespace AndOS.Infrastructure.Managers;

internal class ProgramManager : IProgramManager
{
    private readonly ILogger<ProgramManager> _logger;

    public ProgramManager(ILogger<ProgramManager> logger, IJSRuntime jsRuntime)
    {
        _logger = logger;
        _jsRuntime = jsRuntime;
        _programs.Add(new FileExplorerComponent());
        _programs.Add(new NotepadComponent());
        _programs.Add(new UserConfigurationComponent());
    }

    private readonly IJSRuntime _jsRuntime;
    private readonly List<Assembly> _assemblies = [];
    public List<Program> Programs => _programs.ToList();
    ObservableCollection<Program> _programs { get; } = [];
    public event Func<Program, Task> OnInstall;
    public event Func<Program, Task> OnUninstall;

    public async Task LoadAssembliesAsync()
    {
        try
        {
            List<AssemblyInfo> assemblies = await _jsRuntime.InvokeAsync<List<AssemblyInfo>>(IndexedDbConsts.GetAll, IndexedDbConsts.DbName, IndexedDbConsts.AssemblyStoreIndexedDb);
            foreach (AssemblyInfo assemblyInfo in assemblies)
            {
                Assembly assembly = Assembly.Load(assemblyInfo.Binary);
                await LoadProgramFromAssembly(assembly);
                _logger.LogInformation($"Assembly loaded: {assemblyInfo.Name}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading assemblies from IndexedDB");
        }
    }

    public async Task RemoveExternalAssemblyAsync(Assembly assembly)
    {
        foreach (Program program in _programs.Where(x => x is Program && x.Assembly == assembly)
            .ToList())
        {
            _programs.Remove(program);
            await OnUninstall?.Invoke(program);
        }
        _assemblies.Remove(assembly);
        await RemoveAssemblyOnIndexedDb(assembly);
    }

    public bool VerifyIfExistsAssembly(Assembly assembly)
    {
        Assembly assemblyExisting = _assemblies.FirstOrDefault(x => x.GetName().Name == assembly.GetName().Name);
        return assemblyExisting != null;
    }

    public bool VerifyIfExistsAssembly(Assembly assembly, out Assembly assemblyExisting)
    {
        assemblyExisting = _assemblies.FirstOrDefault(x => x.GetName().Name == assembly.GetName().Name);
        return assemblyExisting != null;
    }

    public ResultCompareVersion CheckVersionAssembly(Assembly oldAssembly, Assembly newAssembly)
    {
        Version versionOldAssembly = oldAssembly.GetName().Version;
        Version versionNewAssembly = newAssembly.GetName().Version;
        if (versionOldAssembly == versionNewAssembly)
            return ResultCompareVersion.Equal;
        else if (versionOldAssembly > versionNewAssembly)
            return ResultCompareVersion.Lower;
        else
            return ResultCompareVersion.Higher;
    }

    public bool IsValidModule(Assembly assembly)
    {
        List<AssemblyMetadataAttribute> attributesAssembly = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().ToList();

        string isValidModule = attributesAssembly.FirstOrDefault(a => a.Key.Equals(AssemblyConsts.TagModule))?.Value;
        _logger.LogInformation(isValidModule?.ToString());
        return isValidModule == "true";
    }

    async Task LoadProgramFromAssembly(Assembly assembly)
    {
        List<Program> tempPrograms = [];
        try
        {
            // Obter todos os tipos no assembly atual
            Type[] types = assembly.GetTypes();
            // Filtrar tipos que herdam de Program
            foreach (Type type in types.Where(t => t.IsAssignableTo(typeof(Program)) && !t.IsAbstract))
            {
                _logger.LogInformation("Name type to instance {0}", type.FullName);
                // Instanciar o tipo
                Program programInstance = (Program)Activator.CreateInstance(type);
                //programInstance.SetAssembly(assembly);
                // Adicionar a instância à coleção
                tempPrograms.Add(programInstance);
            }
            foreach (Program program in tempPrograms)
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
            _logger.LogInformation("{0} programs loaders from assembly: {1}", _programs.Count, assembly.FullName);
        }
    }

    public async Task AddExternalProgramAsync(byte[] assemblybinary)
    {
        Assembly assembly = Assembly.Load(assemblybinary);
        await LoadProgramFromAssembly(assembly);
        await AddAssemblyOnIndexedDb(assembly, assemblybinary);
    }

    public async Task AddExternalProgramAsync(Assembly assembly)
    {

        byte[] binaryData = File.ReadAllBytes(assembly.Location);
        await AddExternalProgramAsync(binaryData);
    }

    public async Task AddExternalProgramAsync(string path)
    {
        FileInfo file = new(path);
        byte[] binaryData = File.ReadAllBytes(file.FullName);
        await AddExternalProgramAsync(binaryData);
    }

    public async Task AddAssemblyOnIndexedDb(Assembly assembly, byte[] assemblybinary)
    {
        AssemblyInfo assemblyInfo = new AssemblyInfo(assembly.GetName().Name, assembly.GetName().Version, assemblybinary);
        await _jsRuntime.InvokeVoidAsync(IndexedDbConsts.Add, IndexedDbConsts.DbName, IndexedDbConsts.AssemblyStoreIndexedDb, assemblyInfo);
    }

    public async Task RemoveAssemblyOnIndexedDb(Assembly assembly)
    {
        await _jsRuntime.InvokeVoidAsync(IndexedDbConsts.Remove, IndexedDbConsts.DbName, IndexedDbConsts.AssemblyStoreIndexedDb, assembly.GetName().Name);
    }
}