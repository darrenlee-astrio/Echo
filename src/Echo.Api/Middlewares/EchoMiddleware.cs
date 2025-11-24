using System.Text;

namespace Echo.Api.Middlewares;

public class EchoMiddleware
{
    private readonly RequestDelegate _next;

    public EchoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Read the request body
        string requestBody;
        using (var reader = new StreamReader(
                   context.Request.Body,
                   Encoding.UTF8,
                   detectEncodingFromByteOrderMarks: false,
                   leaveOpen: true))
        {
            requestBody = await reader.ReadToEndAsync();
        }

        var hasExpectedStatusCode =
            context.Request.Headers.TryGetValue("x-expected-status-code", out var expectedStatusCode);

        if (hasExpectedStatusCode && int.TryParse(expectedStatusCode.ToString(), out var statusCode))
        {
            if (statusCode != StatusCodes.Status200OK)
            {
                context.Response.StatusCode = statusCode;
                return;
            }
        }

        context.Response.StatusCode = StatusCodes.Status200OK;
        context.Response.ContentType = context.Request.ContentType;
        await context.Response.WriteAsync(requestBody ?? string.Empty);
        return;
    }
}