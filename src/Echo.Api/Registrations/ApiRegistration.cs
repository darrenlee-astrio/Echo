using Echo.Api.Middlewares;
using Scalar.AspNetCore;
using Serilog;

namespace Echo.Api.Registrations;

public static class ApiRegistration
{
    internal static IServiceCollection RegisterApi(this IServiceCollection services)
    {
        return services.RegisterOpenApi();
    }

    private static IServiceCollection RegisterOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi();

        return services;
    }

    internal static IApplicationBuilder UseApi(this WebApplication app)
    {
        //app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();

        app.UseWhen(
            ctx => ctx.Request.Path.StartsWithSegments("/api"),
            branch =>
            {
                branch.UseMiddleware<RequestBodyLoggingMiddleware>();
                branch.UseMiddleware<EchoMiddleware>();
            });

        app.UseOpenApiPage();

        app.MapGet("hello", () => "Hello world");

        return app;
    }

    private static WebApplication UseOpenApiPage(this WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference("/docs");

        return app;
    }
}