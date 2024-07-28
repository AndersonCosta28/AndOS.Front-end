using AndOS.Application.Records;
using AndOS.Module.FileExplorer.Dialogs.Save;
using AndOS.Module.FileExplorer.Dialogs.Select.Files;

namespace AndOS.Module.FileExplorer.Dialogs;

public class FileExplorerDialogs(IDialogService dialogService, IToastService toastService, ILogger<FileExplorerDialogs> logger) : IFileExplorerDialogs
{
    public async Task SaveAsAsync(FileDTO file, string text)
    {
        var data = new DialogSaveParams()
        {
            File = file,
            Value = text,
        };
        var parameters = new DialogParameters()
        {
            Width = "70%",
            Height = "70%",
            Title = "Save as"
        };
        await dialogService.ShowDialogAsync<DialogSaveFileExplorerBase>(data, parameters);
    }

    public async Task<SelectedFile> SelectFileAsync()
    {
        var parameters = new DialogParameters()
        {
            Title = $"Select folder",
            Modal = false,
            Width = "70%",
            Height = "70%",
            PreventScroll = true
        };

        var dialogReference = await dialogService.ShowDialogAsync<DialogSelectFile>(parameters);
        var result = await dialogReference.Result;

        if (!result.Cancelled && result.Data is SelectedFile file)
            return file;
        else
        {
            var message = "Nenhum item foi selecionada";
            logger.Log(LogLevel.Debug, message);
            toastService.ShowError(message);
            return null;
        }
    }

    public Task<List<SelectedFile>> SelectFilesAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<SelectedFolder> SelectFolderAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<List<SelectedFolder>> SelectFoldersAsync()
    {
        throw new System.NotImplementedException();
    }
}