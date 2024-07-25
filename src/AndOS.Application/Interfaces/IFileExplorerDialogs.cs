using AndOS.Application.Records;

namespace AndOS.Application.Interfaces;

public interface IFileExplorerDialogs
{
    Task SaveAsAsync(FileDTO file, string text);
    Task<SelectedFile> SelectFileAsync();
    Task<List<SelectedFile>> SelectFilesAsync();

    Task<SelectedFolder> SelectFolderAsync();
    Task<List<SelectedFolder>> SelectFoldersAsync();
}