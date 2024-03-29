using dotNetBackend.Middlewares;
using dotNetBackend.models.DbFirst;
using dotNetBackend.Services;
using dotNetBackend.Servises;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "CORS";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy
                           .AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                      });
});


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

RedisManager.ConnectionString = builder.Configuration.GetConnectionString("RedisConnection");

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
        ClockSkew = new TimeSpan(0, 0, TimeSpanSecond),
        ValidateLifetime = false
    };
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<NewContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IRequestService, RequestService>();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
