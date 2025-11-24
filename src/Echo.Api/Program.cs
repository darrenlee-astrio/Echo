using Echo.Api.Registrations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var host = builder.Host;
var services = builder.Services;
var configuration = builder.Configuration;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    host.RegisterSerilog(configuration);

    services.RegisterApi();

    var app = builder.Build();

    app.UseApi();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An error occurred while running the application.");

    // In order for the Windows Service Management system to leverage configured
    // recovery options, we need to terminate the process with a non-zero exit code.
    Environment.Exit(1);
}
finally
{
    await Log.CloseAndFlushAsync();
}