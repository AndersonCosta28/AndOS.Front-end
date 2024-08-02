using AndOS.Application.Extensions;
using AndOS.Application.Interfaces;
using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.Files.Create;
using AndOS.Shared.Requests.Files.Delete;
using AndOS.Shared.Requests.Files.Get.GetById;
using AndOS.Shared.Requests.Files.Get.GetByPath;
using AndOS.Shared.Requests.Files.Update.Content;
using AndOS.Shared.Requests.Files.Update.Rename;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Net.Http.Json;

namespace AndOS.Infrastructure.Api;

internal class FileService(HttpClient httpClient,
    ILogger<FileService> logger,
    ICloudStorageServiceFactory cloudStorageServiceFactory,
    IToastService toastService,
    IDialogService dialogService) : IFileService
{
    public event Func<Task> OnFileCreated;
    public event Func<Task> OnFileUpdated;
    public event Func<Task> OnFileDeleted;
    const string _endpoint = "Files";

    public async Task<CreateFileResponse> CreateAsync(CreateFileRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"{_endpoint}", request, cancellationToken: cancellationToken);
        await response.HandleResponse(cancellationToken);
        if (OnFileCreated != null)
            await OnFileCreated?.Invoke();
        var result = await response.Content.ReadFromJsonAsync<CreateFileResponse>(cancellationToken: cancellationToken);
        return result;
    }

    public async Task<UpdateContentFileResponse> UpdateContentFileAsync(UpdateContentFileRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"{_endpoint}/UpdateContent", request, cancellationToken: cancellationToken);
        await response.HandleResponse(cancellationToken);
        if (OnFileUpdated != null)
            await OnFileUpdated?.Invoke();
        var result = await response.Content.ReadFromJsonAsync<UpdateContentFileResponse>(cancellationToken: cancellationToken);
        return result;
    }

    public async Task RenameAsync(RenameFileRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"{_endpoint}/rename", request, cancellationToken: cancellationToken);
        await response.HandleResponse(cancellationToken);
        if (OnFileUpdated != null)
            await OnFileUpdated?.Invoke();
    }

    public async Task DeleteAsync(DeleteFileRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"{_endpoint}?{request.ToQueryString()}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        if (OnFileDeleted != null)
            await OnFileDeleted?.Invoke();
    }

    public async Task<GetFileByIdResponse> GetByIdAsync(GetFileByIdRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"{_endpoint}/GetById?{request.ToQueryString()}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<GetFileByIdResponse>(cancellationToken: cancellationToken);
        return result;
    }

    public async Task<GetFileByPathResponse> GetByPathAsync(GetFileByPathRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"{_endpoint}/GetByPath?{request.ToQueryString()}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<GetFileByPathResponse>(cancellationToken: cancellationToken);
        return result;
    }

    public async Task SaveAsync(FileDTO file, AndOS.Core.Enums.CloudStorage cloudStorage, string content, CancellationToken cancellationToken = default)
    {
        try
        {
            string url;
            if (file.Id == Guid.Empty)
            {
                var response = await CreateAsync(new(extension: file.Extension, name: file.Name, parentFolderId: file.ParentFolder.Id, size: file.Size), cancellationToken);
                url = response.Url;
            }
            else
            {
                var response = await UpdateContentFileAsync(new(id: file.Id), cancellationToken);
                url = response.Url;
            }

            var service = cloudStorageServiceFactory.Create(cloudStorage);
            await service.UploadAsync(url, content, cancellationToken);
            toastService.ShowSuccess("File saved successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, null);
            toastService.ShowError(ex.Message);
        }
    }
}