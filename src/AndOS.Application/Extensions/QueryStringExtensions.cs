namespace AndOS.Application.Extensions;

public static class QueryStringExtensions
{
    public static string ToQueryString(this object request)
    {
        var properties = request.GetType().GetProperties()
            .Where(prop => prop.GetValue(request, null) != null)
            .Select(prop => $"{Uri.EscapeDataString(prop.Name)}={Uri.EscapeDataString(prop.GetValue(request, null).ToString())}");

        var result = string.Join("&", properties);
        return result;
    }
}