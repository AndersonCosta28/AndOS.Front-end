namespace AndOS.Infrastructure.Http;
public class ProgressMessageHandler : DelegatingHandler
{
    Action<long, long> _progressCallBackDefault = (progress, total) => Console.WriteLine($"Download progress: {progress} bytes / {total} bytes");

    private readonly Action<long, long> _onProgress;

    public ProgressMessageHandler(Action<long, long> onProgress = default) : base(new HttpClientHandler())
    {
        if (onProgress == default)
            _onProgress = _progressCallBackDefault;
        else
            _onProgress = onProgress;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        // Simulação de upload
        if ((request.Method == HttpMethod.Post || request.Method == HttpMethod.Put) &&
           request.Content.Headers.ContentType != null)
        {
            var contentLength = request.Content.Headers.ContentLength.Value;
            var totalBytesSent = 0L;

            using var contentStream = await request.Content.ReadAsStreamAsync();
            using var outputStream = new MemoryStream();
            await contentStream.CopyToAsync(    outputStream, 4096, cancellationToken);
            totalBytesSent = outputStream.Length;

            _onProgress(totalBytesSent, contentLength);
        }

        // Simulação de download
        else if (response.Content != null && response.Content.Headers.ContentLength.HasValue)
        {
            var contentLength = response.Content.Headers.ContentLength.Value;
            var totalBytesReceived = 0L;

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var inputStream = new MemoryStream();
            await responseStream.CopyToAsync(inputStream, 4096, cancellationToken);
            totalBytesReceived = inputStream.Length;

            _onProgress(totalBytesReceived, contentLength);
        }

        return response;
    }
}
