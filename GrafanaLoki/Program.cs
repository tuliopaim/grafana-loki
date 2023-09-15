using Microsoft.AspNetCore.Mvc;
using Serilog;

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
    var currentUserId = Guid.NewGuid();

    logger.LogInformation("Hello World! {CurrentUserId}", currentUserId);

    return Results.Ok();
});

app.MapPost("/", (
    Input body,
    [FromServices]ILogger<Program> logger) =>
{
    var currentUserId = Guid.NewGuid();

    logger.LogInformation(
        "User {CurrentUserId} sent @{Body}", currentUserId, body);

    return Results.Ok();
});

app.MapGet("/divide/{num1}/{num2}", (
    decimal num1,
    decimal num2,
    [FromServices]ILogger<Program> logger) =>
{
    var currentUserId = Guid.NewGuid();

    try
    {
        var result = num1 / num2;
        logger.LogInformation(
            "{CurrentUserId} dividing {num1}/{num2}: {result}",
            currentUserId,
            num1,
            num2,
            result);

        return Results.Ok(result);
    }
    catch (DivideByZeroException)
    {
        logger.LogError(
            "{CurrentUserId} trying to divide by 0",
            currentUserId);

        return Results.BadRequest();
    }
    catch(Exception ex)
    {
        logger.LogError(
            ex,
            "Exception captured, {CurrentUserId} was dividing {num1}/{num2}",
            currentUserId,
            num1,
            num2);

        return Results.BadRequest();
    }

});

app.Run();

record Input(string Name, int Age);
