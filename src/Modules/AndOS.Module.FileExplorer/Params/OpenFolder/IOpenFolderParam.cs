using AndOS.Module.FileExplorer.Enums;

namespace AndOS.Module.FileExplorer.Params.OpenFolder;

public interface IOpenFolderParams
{
    public bool ClearNavigationHistory { get; set; }
    public bool ClearFowardHistory { get; set; }
    public NavigationType NavigationType { get; set; }
}