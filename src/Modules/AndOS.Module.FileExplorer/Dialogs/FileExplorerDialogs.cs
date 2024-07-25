using AndOS.Application.Records;
using AndOS.Module.FileExplorer.Dialogs.Save;
using AndOS.Module.FileExplorer.Dialogs.Select.Files;

namespace AndOS.Module.FileExplorer.Dialogs;

public class FileExplorerDialogs(IDialogService dialogService, IToastService toastService, ILogger<FileExplorerDialogs> logger) : IFileExplorerDialogs
{
    public async Task SaveAsAsync(FileDTO file, string text)
    {
        DialogSaveParams data = new()
        {
            File = file,
            Value = text,
        };
        DialogParameters parameters = new()
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

        IDialogReference dialogReference = await dialogService.ShowDialogAsync<DialogSelectFile>(parameters);
        DialogResult result = await dialogReference.Result;

        if (!result.Cancelled && result.Data is SelectedFile file)
            return file;
        else
        {
            string message = "Nenhum item foi selecionada";
            logger.LogInformation(message);
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