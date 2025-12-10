using DotNetEnv;
using groundhog_tasks_service.Services;
using groundhog_tasks_service.Jobs;
using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Quartz;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    $"Host={Env.GetString("POSTGRES_HOST")};" +
    $"Port={Env.GetString("POSTGRES_PORT")};" +
    $"Database={Env.GetString("POSTGRES_DB")};" +
    $"Username={Env.GetString("POSTGRES_USER")};" +
    $"Password={Env.GetString("POSTGRES_PASSWORD")}";

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

dataSourceBuilder.MapEnum<UserAssignmentStatus>();

var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dataSource));


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("AssignmentReminderJob");

    q.AddJob<AssignmentReminderJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("AssignmentReminderJob-trigger")
        .StartAt(DateTimeOffset.UtcNow.AddSeconds(5))
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(5)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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