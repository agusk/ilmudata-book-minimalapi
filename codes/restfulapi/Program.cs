var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// In-memory data store
var items = new List<string>();
for (int i = 1; i <= 5; i++)
{
    items.Add($"Item {i}");
}

// GET endpoint
app.MapGet("/items", () => items);

// POST endpoint
app.MapPost("/items", (string item) =>
{
    items.Add(item);
    return Results.Created($"/items/{items.Count - 1}", item);
});

// PUT endpoint
app.MapPut("/items/{id}", (int id, string item) =>
{
    if (id < 0 || id >= items.Count)
    {
        return Results.NotFound();
    }
    items[id] = item;
    return Results.NoContent();
});

// DELETE endpoint
app.MapDelete("/items/{id}", (int id) =>
{
    if (id < 0 || id >= items.Count)
    {
        return Results.NotFound();
    }
    items.RemoveAt(id);
    return Results.Ok();
});

app.Run();

