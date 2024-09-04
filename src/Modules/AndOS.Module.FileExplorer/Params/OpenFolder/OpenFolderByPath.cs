using AndOS.Module.FileExplorer.Enums;

namespace AndOS.Module.FileExplorer.Params.OpenFolder;

public class OpenFolderByPath : IOpenFolderParams
{
    public string Path { get; set; }
    public bool ClearNavigationHistory { get; set; }
    public bool ClearFowardHistory { get; set; }
    public NavigationType NavigationType { get; set; }

    public OpenFolderByPath(string path,
        NavigationType navigationType = NavigationType.Default,
        bool clearNavigationHistory = false,
        bool clearFowardHistory = false)
    {
        this.Path = path;
        this.NavigationType = navigationType;
        this.ClearNavigationHistory = clearNavigationHistory;
        this.ClearFowardHistory = clearFowardHistory;
    }
}