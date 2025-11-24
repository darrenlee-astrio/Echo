using System.Text;

namespace Echo.Api.Middlewares;

public class RequestBodyLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestBodyLoggingMiddleware> _logger;

    public RequestBodyLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestBodyLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow the body stream to be read multiple times
        context.Request.EnableBuffering();

        // Read the body
        context.Request.Body.Position = 0;
        using var reader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();

        // Reset position so the rest of the pipeline can read it
        context.Request.Body.Position = 0;

        if (!string.IsNullOrEmpty(body))
        {
            _logger.LogInformation("Request {Method} {Path} \nBody: \n{Body}",
                context.Request.Method,
                context.Request.Path,
                body);
        }

        await _next(context);
    }
}