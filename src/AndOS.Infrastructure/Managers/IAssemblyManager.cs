namespace AndOS.Infrastructure.Managers;
public interface IAssemblyManager
{
    Task Add(Assembly assembly, byte[] assemblybinary);
    Task Remove(Assembly assembly);
    Task<List<AssemblyInfo>> GetAll();
}
