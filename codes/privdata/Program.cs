using privdata.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

// Add data protection services
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AppDbContext>();

// add custom data protection provider
builder.Services.AddTransient<IDataProtectionProvider, SqlServerDataProtectionProvider>();
builder.Services.AddTransient<SensitiveDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/employees", (AppDbContext dbContext, 
    SensitiveDataService service, Employee employee) =>
{
    dbContext.Employees.Add(service.EncryptEmployeeData(employee));
    dbContext.SaveChanges();
    return Results.Ok();
});

app.MapGet("/employees", (AppDbContext dbContext, 
    SensitiveDataService service) =>
{
    var employees = dbContext.Employees.AsEnumerable()
        .Select(service.MaskEmployeeData).ToList();
    return Results.Ok(employees);
});

app.Run();
