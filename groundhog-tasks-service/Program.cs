using DotNetEnv;
using GroundhogTasksService.Data;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    $"Host={Env.GetString("POSTGRES_HOST")};" +
    $"Port={Env.GetString("POSTGRES_PORT")};" +
    $"Database={Env.GetString("POSTGRES_DB")};" +
    $"Username={Env.GetString("POSTGRES_USER")};" +
    $"Password={Env.GetString("POSTGRES_PASSWORD")}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
