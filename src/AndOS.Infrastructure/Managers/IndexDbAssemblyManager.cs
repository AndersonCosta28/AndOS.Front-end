using Microsoft.JSInterop;

namespace AndOS.Infrastructure.Managers;
internal class IndexDbAssemblyManager(IJSRuntime jSRuntime) : IAssemblyManager
{
    const string _dbName = "AndOS.Db";
    const string _getAll = "indexedDBFunctions.getAll";
    const string _add = "indexedDBFunctions.add";
    const string _remove = "indexedDBFunctions.remove";
    const string _open = "indexedDBFunctions.open";
    const string _assemblyStoreIndexedDb = "AssembliesStore";

    public async Task Add(Assembly assembly, byte[] assemblybinary)
    {
        var assemblyInfo = new AssemblyInfo(assembly.GetName().Name, assembly.GetName().Version, assemblybinary);
        await jSRuntime.InvokeVoidAsync(_add, _dbName, _assemblyStoreIndexedDb, assemblyInfo);
    }

    public async Task<List<AssemblyInfo>> GetAll()
    {
        var result = await jSRuntime.InvokeAsync<List<AssemblyInfo>>(_getAll, _dbName, _assemblyStoreIndexedDb);
        return result;
    }
    public async Task Remove(Assembly assembly)
    {
        await jSRuntime.InvokeVoidAsync(_remove, _dbName, _assemblyStoreIndexedDb, assembly.GetName().Name);
    }
}
