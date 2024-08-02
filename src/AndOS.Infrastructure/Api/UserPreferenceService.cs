using AndOS.Application.Extensions;
using AndOS.Application.Interfaces;
using AndOS.Shared.DTOs;
using AndOS.Shared.Requests.UserPreferences.Delete;
using AndOS.Shared.Requests.UserPreferences.Get.GetDefaultProgramByExtension;
using AndOS.Shared.Requests.UserPreferences.Update;
using System.Net.Http.Json;

namespace AndOS.Infrastructure.Api;

public class UserPreferenceService(HttpClient httpClient) : IUserPreferenceService
{
    public async Task<UserPreferenceDTO> GetUserPreferenceAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"userpreferences/getuserpreferences", cancellationToken);
        await response.HandleResponse(cancellationToken);
        return await response.Content.ReadFromJsonAsync<UserPreferenceDTO>(cancellationToken);
    }

    public async Task UpdateLanguageAsync(UpdateLanguageRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync("userpreferences/UpdateLanguage", request, cancellationToken);
        await response.HandleResponse(cancellationToken);
    }

    public async Task UpdateDefaultProgramToExtensionAsync(UpdateDefaultProgramsToExtensionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync("userpreferences/UpdateDefaultProgramsToExtension", request, cancellationToken);
        await response.HandleResponse(cancellationToken);
    }

    public async Task<GetDefaultProgramByExtensionResponse> GetDefaultProgramByExtensionAsync(GetDefaultProgramByExtensionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync($"userpreferences/GetDefaultProgramByExtension?Extension={request.Extension}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        return await response.Content.ReadFromJsonAsync<GetDefaultProgramByExtensionResponse>(cancellationToken = default);
    }

    public async Task DeleteDefaultProgramToExtensionAsync(DeleteDefaultProgramToExtensionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"userpreferences/DeleteDefaultProgramsToExtension?extension={request.Extension}", cancellationToken);
        await response.HandleResponse(cancellationToken);
    }
}
