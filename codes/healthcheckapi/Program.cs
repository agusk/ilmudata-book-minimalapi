using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks
// builder.Services.AddHealthChecks()
//     .AddCheck("MyApp", () =>
//         HealthCheckResult.Healthy("The check of the sample is OK!"),
//         tags: ["myapp"]);

// Add health checks with database
// builder.Services.AddHealthChecks()
//     .AddSqlServer(builder.Configuration["ConnectionStrings:MyDB"] ?? "", 
//     name: "SQL Server", 
//     tags: new[] { "db", "sql", "sqlserver" });

//////////Custom health check
builder.Services.AddHealthChecks()
    .AddCheck<ExternalEndpointHealthCheck>("ExternalEndpointHealthCheck", 
        null, 
        new[] { "external_endpoint" });

builder.Services.AddSingleton<ExternalEndpointHealthCheck>(_ => 
    new ExternalEndpointHealthCheck("https://www.google.com"));
//////////Custom health check

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map health check endpoints
app.MapHealthChecks("/health");

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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
