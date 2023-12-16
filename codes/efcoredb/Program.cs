using EFCoreDb.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => {
    var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
    if (useInMemoryDb)
    {
        options.UseInMemoryDatabase("TrainingDB");
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"));
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/products", async (AppDbContext dbContext) =>
    await dbContext.Products.ToListAsync());

app.MapGet("/products/{id}", async (AppDbContext dbContext, int id) =>
{
    var product = await dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
    return Results.Ok(product);
});

app.MapPost("/products", async (AppDbContext dbContext, Product product) =>
{
    dbContext.Products.Add(product);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{id}", async (AppDbContext dbContext, Product product, int id) =>
{
    var p = await dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
    if(p != null)
    {
        p.Price = product.Price;
        if(!string.IsNullOrEmpty(product.Name))
            p.Name = product.Name;
        
        dbContext.Products.Update(p);
        await dbContext.SaveChangesAsync();
    }
    return Results.Ok(p);
});

app.MapDelete("/products/{id}", async (AppDbContext dbContext, int id) =>
{
    var product = await dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
    if (product != null)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
    }
    return Results.Ok(product);
});






app.Run();
