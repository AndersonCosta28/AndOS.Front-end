using AndOS.Application.Extensions;
using AndOS.Application.Interfaces;
using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.Folders.Create;
using AndOS.Shared.Requests.Folders.Delete;
using AndOS.Shared.Requests.Folders.Get.GetById;
using AndOS.Shared.Requests.Folders.Get.GetByPath;
using AndOS.Shared.Requests.Folders.Update.Rename;
using System.Net.Http.Json;
using System.Threading;

namespace AndOS.Infrastructure.Api;

internal class FolderService(HttpClient httpClient) : IFolderService
{
    private readonly HttpClient _httpClient = httpClient;
    public event Func<Task> OnFolderCreated;
    public event Func<Task> OnFolderUpdated;
    public event Func<Task> OnFolderDeleted;

    public async Task CreateAsync(CreateFolderRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Folders", request, cancellationToken);
        await response.HandleResponse(cancellationToken);
        await OnFolderCreated?.Invoke();
    }

    public async Task RenameAsync(RenameFolderRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.PutAsJsonAsync("Folders/Rename", request, cancellationToken);
        await response.HandleResponse(cancellationToken);
        await OnFolderUpdated?.Invoke();
    }

    public async Task DeleteAsync(DeleteFolderRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"Folders?{request.ToQueryString()}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        await OnFolderDeleted?.Invoke();
    }

    public async Task<GetFolderByIdResponse> GetByIdAsync(GetFolderByIdRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"Folders/GetById?{request.ToQueryString()}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        return await response.Content.ReadFromJsonAsync<GetFolderByIdResponse>(cancellationToken: cancellationToken);
    }

    public async Task<GetFolderByPathResponse> GetByPathAsync(GetFolderByPathRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"Folders/GetByPath?{request.ToQueryString()}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        return await response.Content.ReadFromJsonAsync<GetFolderByPathResponse>(cancellationToken: cancellationToken);
    }
}