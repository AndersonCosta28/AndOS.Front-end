using AndOS.Module.FileExplorer.Params;

namespace AndOS.Module.FileExplorer.Dialogs.Save;
public class DialogSaveParams : IFileExplorerParams
{
    public FileDTO File { get; set; }
    public string Value { get; set; }
    public bool New { get; set; }
}