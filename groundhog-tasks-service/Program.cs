using DotNetEnv;
//using groundhog_tasks_service.Data;
using GroundhogTasksService.Data;
using Microsoft.EntityFrameworkCore;

Env.Load(); // carga las variables del .env

var builder = WebApplication.CreateBuilder(args);

// Construir la cadena de conexión desde .env
var connectionString =
    $"Host={Env.GetString("POSTGRES_HOST")};" +
    $"Port={Env.GetString("POSTGRES_PORT")};" +
    $"Database={Env.GetString("POSTGRES_DB")};" +
    $"Username={Env.GetString("POSTGRES_USER")};" +
    $"Password={Env.GetString("POSTGRES_PASSWORD")}";

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
