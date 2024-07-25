namespace AndOS.Application.Extensions;

public static class HttpResponseMessageExtension
{
    public static async Task HandleResponse(this HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var message = string.Empty;

        if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            var errorDto = await response.Content.ReadFromJsonAsync<ErrorDTO>(cancellationToken: cancellationToken);
            message = $"{errorDto.Title}\n{errorDto.Detail}";
        }
        else
            message = await response.Content.ReadAsStringAsync(cancellationToken);

        Console.WriteLine("Error on request");
        Console.WriteLine(message);
        throw new HttpRequestException(message);
    }
}