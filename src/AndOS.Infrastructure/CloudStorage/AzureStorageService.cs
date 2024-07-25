using AndOS.Application.Interfaces;
using AndOS.Shared.Consts;
using Polly;
using Polly.Retry;
namespace AndOS.Infrastructure.CloudStorage;

public class AzureStorageService : ICloudStorageService
{
    public async Task UploadAsync(string url, string data, CancellationToken cancellationToken = default)
    {

        var optionsRetry = new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(3),
        };

        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(optionsRetry)
            //.AddTimeout(TimeSpan.FromSeconds(1))
            .Build();

        await pipeline.ExecuteAsync(async token =>
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
            await Task.Delay(1000, token);
            var response = await client.SendAsync(request, token);
            response.EnsureSuccessStatusCode();
        }, cancellationToken);
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