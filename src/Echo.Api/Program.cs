using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger());

var app = builder.Build();

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

app.MapGet("hello", () =>
{
    return Results.Ok("Hello world");
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.Run();