using Asp.Versioning;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var host = builder.Host;
var services = builder.Services;
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

host.UseSerilog(Log.Logger);

services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath | HttpLoggingFields.RequestBody;
    options.RequestBodyLogLimit = 1024;
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
});


var app = builder.Build();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .Build();

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/swagger"),
    appBuilder => appBuilder.UseHttpLogging());

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        using (var reader = new StreamReader(context.Request.Body))
        {
            var requestBody = await reader.ReadToEndAsync();

            var hasExpectedStatusCode = context.Request.Headers.TryGetValue("x-expected-status-code", out var expectedStatusCode);

            if (hasExpectedStatusCode)
            {
                int statusCode = int.Parse(expectedStatusCode.ToString());

                if (statusCode != StatusCodes.Status200OK)
                {
                    context.Response.StatusCode = statusCode;
                    return;
                }
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = context.Request.ContentType;
            await context.Response.WriteAsync(requestBody);
            return;
        }
    }
    await next();
});

app.MapGet("hello", () => "Hello world")
   .WithApiVersionSet(versionSet)
   .MapToApiVersion(1);

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();