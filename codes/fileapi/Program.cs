using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAntiforgery();

var app = builder.Build();
app.UseAntiforgery();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var fileApi = app.MapGroup("/api/file");
fileApi.MapPost("/upload", async (IFormFile file,[FromForm]string description) =>
{
    var uploadsFolderPath = Path.Combine(app.Environment.WebRootPath, "uploads");
    Directory.CreateDirectory(uploadsFolderPath);
    var filePath = Path.Combine(uploadsFolderPath, file.FileName);

    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    return Results.Ok(new { FilePath = $"/Uploads/{file.FileName}", Description = description });
}).DisableAntiforgery();

fileApi.MapGet("/download/{fileName}", async (string fileName) =>
{
    var filePath = Path.Combine(app.Environment.WebRootPath, "Uploads", fileName);
    if (!File.Exists(filePath))
    {
        return Results.NotFound("File not found.");
    }

    var memoryStream = new MemoryStream();
    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
    {
        await stream.CopyToAsync(memoryStream);
    }
    memoryStream.Position = 0;
    return Results.File(memoryStream, "application/octet-stream", fileName);
});

app.Run();
