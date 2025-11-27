using DotNetEnv;
using GroundhogTasksService.Data;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Construir la cadena de conexión desde .env
var connectionString =
    $"Host={Env.GetString("DB_HOST")};" +
    $"Port={Env.GetString("DB_PORT")};" +
    $"Database={Env.GetString("DB_NAME")};" +
    $"Username={Env.GetString("DB_USER")};" +
    $"Password={Env.GetString("DB_PASS")}";

// Registrar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
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
app.UseAuthorization();
app.MapControllers();

app.Run();
