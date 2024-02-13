using dotNetBackend.models;
using dotNetBackend.models.DbFirst;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<NewContext>(options => options.UseNpgsql(connectionString));

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

app.UseAuthorization();

app.MapControllers();

app.Run();
