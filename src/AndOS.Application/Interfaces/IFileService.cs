using AndOS.Shared.Requests.Files.Create;
using AndOS.Shared.Requests.Files.Delete;
using AndOS.Shared.Requests.Files.Get.GetById;
using AndOS.Shared.Requests.Files.Get.GetByPath;
using AndOS.Shared.Requests.Files.Update.Content;
using AndOS.Shared.Requests.Files.Update.Rename;

namespace AndOS.Application.Interfaces;

public interface IFileService : IService
{
    public event Func<Task> OnFileCreated;
    public event Func<Task> OnFileUpdated;
    public event Func<Task> OnFileDeleted;
    Task<CreateFileResponse> CreateAsync(CreateFileRequest request, CancellationToken cancellationToken = default);
    Task<GetFileByIdResponse> GetByIdAsync(GetFileByIdRequest request, CancellationToken cancellationToken = default);
    Task<GetFileByPathResponse> GetByPathAsync(GetFileByPathRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default);

    Task RenameAsync(RenameFileRequest request, CancellationToken cancellationToken = default);

    Task<UpdateContentFileResponse> UpdateContentFileAsync(UpdateContentFileRequest request, CancellationToken cancellationToken = default);
}