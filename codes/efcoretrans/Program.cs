using efcoretrans.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/createorder", async (AppDbContext dbContext, Order order) =>
{
    using var transaction = await dbContext.Database.BeginTransactionAsync();
    try
    {
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        await transaction.CommitAsync();
        return Results.Ok(order);
    }
    catch
    {
        // Rollback transaction if there are any exceptions
        await transaction.RollbackAsync();
        throw;
    }
});

app.Run();

