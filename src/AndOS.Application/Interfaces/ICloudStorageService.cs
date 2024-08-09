namespace AndOS.Application.Interfaces;

public interface ICloudStorageService
{
    Task UploadAsync(string url, string data, Action<long, long> progressCallBack = default, CancellationToken cancellationToken = default);
    Task UploadAsync(string url, Stream stream, Action<long, long> progressCallBack = default, CancellationToken cancellationToken = default);
    Task<string> DownloadStringAsync(string url, Action<long, long> progressCallBack = default, CancellationToken cancellationToken = default);
    Task<Stream> DownloadStreamAsync(string url, Action<long, long> progressCallBack = default, CancellationToken cancellationToken = default);
}