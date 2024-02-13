using dotNetBackend.models.DbFirst;
using dotNetBackend.Services;
using dotNetBackend.Servises;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Modification of Swagger for authentication
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string> ()
        }
    });
});


//builder.Services.AddStackExchangeRedisCache(option =>
//{
//    //var connection 
//    option.Configuration = "localhost:6379";
//});


// Configuring Authentication
var secret = builder.Configuration["JWTTokenSettings:Secret"] ?? throw new InvalidOperationException("Secret not configured");
var TimeSpanSecond = builder.Configuration.GetValue<int>("JWTTokenSettings:TimeSpanSecond");
builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
    jwtOptions.RequireHttpsMetadata = false; // Develop 
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = new TimeSpan(0, 0, TimeSpanSecond)
    };
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<NewContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IRequestService, RequestService>();

var app = builder.Build();

//// Creating auto migrations
//using var serviceScope = app.Services.CreateScope();
//var dbContext = serviceScope.ServiceProvider.GetService<NewContext>();
//dbContext?.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect($"127.0.0.1:6379");
//var db = _redis.GetDatabase();
//db.Ping();



//var db = RedisContext.GetDatabase();
//db.StringSet("key", "value");
//string? value = db.StringGet("key");
//System.Console.WriteLine(value);

app.Run();
