using mongodbapp.Models;
using MongoDB.Driver;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// POST: Add a new product
app.MapPost("/products", async (MongoDbContext dbContext, Product product) =>
{
    await dbContext.Products.InsertOneAsync(product);
    return Results.Created($"/products/{product.Id}", product);
});

// GET: Retrieve all products
app.MapGet("/products", async (MongoDbContext dbContext) =>
    await dbContext.Products.Find(product => true).ToListAsync());

// GET: Retrieve a single product by ID
app.MapGet("/products/{id}", async (MongoDbContext dbContext, string id) =>
{
    var product = await dbContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

// PUT: Update a product
app.MapPut("/products/{id}", async (MongoDbContext dbContext, string id, Product updatedProduct) =>
{
    var result = await dbContext.Products.ReplaceOneAsync(p => p.Id == id, updatedProduct);
    return result.IsAcknowledged && result.ModifiedCount > 0 ? Results.NoContent() : Results.NotFound();
});

// DELETE: Delete a product
app.MapDelete("/products/{id}", async (MongoDbContext dbContext, string id) =>
{
    var result = await dbContext.Products.DeleteOneAsync(p => p.Id == id);
    return result.IsAcknowledged && result.DeletedCount > 0 ? Results.Ok() : Results.NotFound();
});



app.Run();

