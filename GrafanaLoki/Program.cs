using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddLogging();
builder.Services.AddSerilog();

var app = builder.Build();

app.MapGet("/", ([FromServices]ILogger<Program> logger) =>
{
    logger.LogInformation("Hello World!");
    return Results.Ok();
});

app.Run();
