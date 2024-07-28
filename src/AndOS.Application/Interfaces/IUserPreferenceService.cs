using AndOS.Shared.Requests.UserPreferences.Delete;
using AndOS.Shared.Requests.UserPreferences.Get.GetDefaultProgramByExtension;
using AndOS.Shared.Requests.UserPreferences.Update;

namespace AndOS.Application.Interfaces;

public interface IUserPreferenceService
{
    Task<GetDefaultProgramByExtensionResponse> GetDefaultProgramByExtensionAsync(GetDefaultProgramByExtensionRequest request, CancellationToken cancellationToken = default);
    Task<UserPreferenceDTO> GetUserPreferenceAsync(CancellationToken cancellationToken = default);
    Task UpdateLanguageAsync(UpdateLanguageRequest request, CancellationToken cancellationToken = default);
    Task UpdateDefaultProgramToExtensionAsync(UpdateDefaultProgramsToExtensionRequest request, CancellationToken cancellationToken = default);
    Task DeleteDefaultProgramToExtensionAsync(DeleteDefaultProgramToExtensionRequest request, CancellationToken cancellationToken = default);
}
