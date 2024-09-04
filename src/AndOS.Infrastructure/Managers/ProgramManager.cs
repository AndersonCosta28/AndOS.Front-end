using AndOS.Domain.Consts;
using AndOS.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace AndOS.Infrastructure.Managers;

internal class ProgramManager : IProgramManager
{
    private readonly ILogger<ProgramManager> _logger;
    private readonly IAssemblyManager _assemblyManager;

    public ProgramManager(ILogger<ProgramManager> logger, IAssemblyManager assemblyManager)
    {
        this._logger = logger;
        this._assemblyManager = assemblyManager;
        this._programs.Add(new Module.FileExplorer.FileExplorer());
        this._programs.Add(new Module.Notepad.Notepad());
        this._programs.Add(new Module.UserConfiguration.UserConfiguration());
        this._programs.Add(new Module.VideoPlayer.VideoPlayer());
        this._programs.Add(new Module.MusicPlayer.MusicPlayer());
        this._programs.Add(new Module.ImageViewer.ImageViewer());
    }

    private readonly List<Assembly> _assemblies = [];
    public List<Program> Programs => this._programs.ToList();
    ObservableCollection<Program> _programs { get; } = [];
    public event Func<Program, Task> OnInstall;
    public event Func<Program, Task> OnUninstall;

    public async Task LoadAssembliesAsync()
    {
        try
        {
            var assemblies = await this._assemblyManager.GetAll();
            foreach (var assemblyInfo in assemblies)
            {
                var assembly = Assembly.Load(assemblyInfo.Binary);
                await this.LoadProgramFromAssembly(assembly);
                this._logger.Log(LogLevel.Debug, $"Assembly loaded: {assemblyInfo.Name}");
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error loading assemblies from IndexedDB");
        }
    }

    public async Task RemoveExternalAssemblyAsync(Assembly assembly)
    {
        foreach (var program in this._programs.Where(x => x is Program && x.Assembly == assembly)
            .ToList())
        {
            this._programs.Remove(program);
            if (OnUninstall != null)
                await OnUninstall?.Invoke(program);
        }
        this._assemblies.Remove(assembly);
        await this._assemblyManager.Remove(assembly);
    }

    public bool VerifyIfExistsAssembly(Assembly assembly)
    {
        var assemblyExisting = this._assemblies.Find(x => x.GetName().Name == assembly.GetName().Name);
        return assemblyExisting != null;
    }

    public bool VerifyIfExistsAssembly(Assembly assembly, out Assembly assemblyExisting)
    {
        assemblyExisting = this._assemblies.Find(x => x.GetName().Name == assembly.GetName().Name);
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
        this._logger.Log(LogLevel.Debug, isValidModule?.ToString());
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
                this._logger.Log(LogLevel.Debug, "Name type to instance {0}", type.FullName);
                var programInstance = (Program)Activator.CreateInstance(type);
                tempPrograms.Add(programInstance);
            }
            foreach (var program in tempPrograms)
            {
                program.IsExternalProgram = true;
                this._programs.Add(program);
                if (OnInstall != null)
                    await OnInstall?.Invoke(program);
            }
            this._assemblies.Add(assembly);
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Tratar exceções de tipo de carga
            Console.WriteLine($"Error on load program from assembly {assembly.FullName}: {ex.Message}");
        }
        finally
        {
            this._logger.Log(LogLevel.Debug, "{0} programs loaders from assembly: {1}", this._programs.Count, assembly.FullName);
        }
    }

    public async Task AddExternalProgramAsync(byte[] assemblybinary)
    {
        var assembly = Assembly.Load(assemblybinary);
        await this.LoadProgramFromAssembly(assembly);
        await this._assemblyManager.Add(assembly, assemblybinary);
    }

    public async Task AddExternalProgramAsync(Assembly assembly)
    {

        var binaryData = File.ReadAllBytes(assembly.Location);
        await this.AddExternalProgramAsync(binaryData);
    }

    public async Task AddExternalProgramAsync(string path)
    {
        FileInfo file = new(path);
        var binaryData = File.ReadAllBytes(file.FullName);
        await this.AddExternalProgramAsync(binaryData);
    }
}