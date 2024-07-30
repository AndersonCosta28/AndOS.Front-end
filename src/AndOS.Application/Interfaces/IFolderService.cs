using AndOS.Shared.Requests.Folders.Create;
using AndOS.Shared.Requests.Folders.Delete;
using AndOS.Shared.Requests.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Get.GetByPath;
using AndOS.Shared.Requests.Folders.Update.Rename;

namespace AndOS.Application.Interfaces;

public interface IFolderService : IService
{
    public event Func<Task> OnFolderCreated;
    public event Func<Task> OnFolderUpdated;
    public event Func<Task> OnFolderDeleted;
    Task CreateAsync(CreateFolderRequest request, CancellationToken cancellationToken = default);
    Task<GetFolderByIdResponse> GetByIdAsync(GetFolderByIdRequest request, CancellationToken cancellationToken = default);
    Task<GetFolderByPathResponse> GetByPathAsync(GetFolderByPathRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteFolderRequest request, CancellationToken cancellationToken = default);
    Task RenameAsync(RenameFolderRequest request, CancellationToken cancellationToken = default);
}