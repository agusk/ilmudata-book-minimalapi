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

var calculatorApi = app.MapGroup("/api/calculator");
calculatorApi.MapPost("/add", (Numeric numbers) => 
{
    var result = numbers.Number1 + numbers.Number2;
    return Results.Ok(new Numeric(numbers.Number1, numbers.Number2, result));
});

calculatorApi.MapPost("/subtract", (Numeric numbers) =>
{
    var result = numbers.Number1 - numbers.Number2;
    return Results.Ok(new Numeric(numbers.Number1, numbers.Number2, result));
});

calculatorApi.MapPost("/multiply", (Numeric numbers) =>
{
    var result = numbers.Number1 * numbers.Number2;
    return Results.Ok(new Numeric(numbers.Number1, numbers.Number2, result));
});

calculatorApi.MapPost("/divide", (Numeric numbers) =>
{
    if (numbers.Number2 == 0)
    {
        return Results.BadRequest("Cannot divide by zero");
    }
    var result = numbers.Number1 / numbers.Number2;
    return Results.Ok(new Numeric(numbers.Number1, numbers.Number2, result));
});

app.Run();

record Numeric(double Number1, double Number2, double Result = 0);