using secrestapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BC = BCrypt.Net.BCrypt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
        }
    }); 
    option.SwaggerDoc("v1", new() { Title = "Minimal Api Demo", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB")));

// configure jwt
var key = builder.Configuration["AppSettings:Secret"];
var keyBytes = Encoding.ASCII.GetBytes(key ?? "aaaaabbbbbcccccddddd11234df4444sd");
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var multiPolicyAuthorization = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();
builder.Services.AddAuthorization(o => o.DefaultPolicy = multiPolicyAuthorization);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/register", async (AppDbContext dbContext, ApiUser usr) =>
{
    var user = new ApiUser
    {
        Username = usr.Username,
        Password = BC.HashPassword(usr.Password),
        Email = usr.Email,
        Name = usr.Name
    };
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/login", (AppDbContext dbContext, IConfiguration configuration, UserLogin model) =>
{
    // ambil user
    var usr = dbContext.Users.Where(o => o.Username == model.UserName).FirstOrDefault();
    if (usr != null)
    {
        if (BC.Verify(model.Password, usr.Password))
        {
            var key = configuration.GetValue<string>("AppSettings:Secret");
            var keyBytes = Encoding.ASCII.GetBytes(key ?? "aaaaabbbbbcccccddddd11234df4444sd");

            // generate token + expired
            var expiredAt = DateTime.Now.AddDays(2);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                        new Claim[]
                        {
                                    new Claim(ClaimTypes.Name,usr.Username)
                        }),
                Expires = expiredAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userToken = new UserToken
            {
                Token = tokenHandler.WriteToken(token),
                ExpiredAt = expiredAt.ToString(),
                Message = ""
            };
            return userToken;
        }
    }

    return new UserToken { Message = "Username or password is invalid" };
});

app.MapGet("/profile", [Authorize] async (HttpContext httpContext, AppDbContext dbContext) =>
{
    var username = httpContext.User.Identity?.Name;
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
    return user != null ? Results.Ok(new {
        user.Username,
        user.Name,
        user.Email
    }) : Results.NotFound();
});


app.Run();

