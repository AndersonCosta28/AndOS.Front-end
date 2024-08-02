using AndOS.Application.Interfaces;
using AndOS.Shared.Consts;
namespace AndOS.Infrastructure.CloudStorage;

public class AzureStorageService : ICloudStorageService
{
    public async Task UploadAsync(string url, string data, CancellationToken cancellationToken = default)
    {
        using HttpClient client = new();
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Language", "pt-BR,pt;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
        request.Headers.Add("x-ms-blob-type", "BlockBlob");
        request.Headers.Add("x-ms-version", AzureConsts.DefaultServiceVersion);

        var content = new StringContent(data, System.Text.Encoding.UTF8, "text/plain");
        //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain", "charset=utf-8");
        request.Content = content;
        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> DownloadAsync(string url, CancellationToken cancellationToken = default)
    {
        using HttpClient httpClient = new();
        HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        return content;
    }
}