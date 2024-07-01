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

var app = builder.Build();


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

            context.Response.ContentType = context.Request.ContentType;
            await context.Response.WriteAsync(requestBody);
            return;
        }
    }
    await next();
});

app.MapGet("hello", () => "Hello world");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();