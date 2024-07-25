namespace AndOS.Application.Interfaces;

public interface ICloudStorageService
{
    Task UploadAsync(string url, string data, CancellationToken cancellationToken = default);
    Task<string> DownloadAsync(string url, CancellationToken cancellationToken = default);
}