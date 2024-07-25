using AndOS.Core.Enums;

namespace AndOS.Application.Interfaces;

public interface ICloudStorageServiceFactory
{
    ICloudStorageService Create(CloudStorage cloudStorageService);
}