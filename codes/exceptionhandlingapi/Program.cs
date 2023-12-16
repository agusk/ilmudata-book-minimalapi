using Serilog;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure Serilog
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Register Serilog
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.UseExceptionHandler("/error");

app.MapGet("/error", (HttpContext httpContext) =>
{
    var exceptionFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionFeature?.Error;

    var problemDetails = new ProblemDetails
    {
        Status = 500,
        Title = "An error occurred while processing your request."
    };

    if (exception is FileNotFoundException)
    {
        problemDetails.Status = 404;
        problemDetails.Title = "File not found.";
    }
    else if (exception is InvalidOperationException)
    {
        problemDetails.Status = 400;
        problemDetails.Title = "Invalid operation.";
    }

    if (app.Logger != null)
    {
        // Logging the exception
        app.Logger?.LogError(exception, "An error occurred: {ErrorMessage}", exception?.Message);
    }
        

    return Results.Problem(problemDetails.Title, statusCode: problemDetails.Status);
});

app.MapGet("/causeinternalerror", () => 
{
    throw new Exception("Internal server error.");
});

app.MapGet("/causefileerror", () => 
{
    throw new FileNotFoundException("Example file not found.");
});

app.MapGet("/causeinvalidoperation", () => 
{
    throw new InvalidOperationException("Invalid operation example.");
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
