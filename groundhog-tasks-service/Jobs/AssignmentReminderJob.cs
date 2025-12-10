using groundhog_tasks_service.Services;
using GroundhogTasksService.Data;
using GroundhogTasksService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace groundhog_tasks_service.Jobs
{
    [DisallowConcurrentExecution]
    public class AssignmentReminderJob : IJob
    {
        private readonly AppDbContext _context;
        private readonly IMailService _mailService;
        private readonly ILogger<AssignmentReminderJob> _logger;

        private static readonly TimeSpan ReminderOffset = TimeSpan.FromHours(1);

        public AssignmentReminderJob(AppDbContext context, IMailService mailService, ILogger<AssignmentReminderJob> logger)
        {
            _context = context;
            _mailService = mailService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;

            var assignments = await _context.Assignments
                .Where(a => a.Cycles > 0)
                .ToListAsync();

            foreach (var assignment in assignments)
            {
                TimeSpan periodicity = assignment.Periodicity ?? TimeSpan.Zero;

                for (int cycle = 1; cycle <= assignment.Cycles; cycle++)
                {
                    long ticks = periodicity.Ticks * (cycle - 1);
                    TimeSpan cycleTimeOffset = new TimeSpan(ticks);
                    DateTime cycleStartDate = assignment.StartDate.Add(cycleTimeOffset);
                    DateTime reminderTime = cycleStartDate.Subtract(ReminderOffset);

                    if (reminderTime <= now && reminderTime > now.Subtract(TimeSpan.FromMinutes(15)))
                    {
                        await ProcessReminderForCycle(assignment, cycle, cycleStartDate);
                    }
                }
            }
        }

        private async Task ProcessReminderForCycle(Assignment assignment, int cycle, DateTime cycleStartDate)
        {
            var userAssignments = await _context.UserAssignments
                .Include(ua => ua.User)
                .Where(ua => ua.AssignmentId == assignment.Id)
                .ToListAsync();

            string subject = $"Recordatorio: Tarea '{assignment.Name}' Pendiente";

            string descContent = string.IsNullOrWhiteSpace(assignment.Description)
                ? "Sin descripción adicional."
                : assignment.Description;

            string htmlBody = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2 style='color: #2c3e50;'>Recordatorio de Tarea (Ciclo {cycle})</h2>
                    <p>Hola,</p>
                    <p>Te recordamos que la tarea <strong>{assignment.Name}</strong> comenzará el <strong>{cycleStartDate:dd/MM/yyyy HH:mm}</strong>.</p>
                    <hr style='border: 1px solid #eee;' />
                    <p><strong>Detalles:</strong></p>
                    <blockquote style='background-color: #f9f9f9; border-left: 4px solid #ccc; padding: 10px; margin: 10px 0;'>
                        {descContent}
                    </blockquote>
                    <p style='font-size: 12px; color: #999;'>Mensaje automático de Groundhog Tasks.</p>
                </div>";

            foreach (var ua in userAssignments)
            {
                if (ua.Status.ToString().ToLower() == "completed") continue;

                string notificationType = $"CYCLE_{cycle}_REMINDER";

                bool alreadyNotified = await _context.Notifications
                    .AnyAsync(n =>
                        n.AssignmentId == assignment.Id &&
                        n.UserId == ua.UserId &&
                        n.Type == notificationType);

                if (!alreadyNotified)
                {
                    try
                    {
                        await _mailService.SendEmailAsync(ua.User.Email, subject, htmlBody);

                        var notification = new Notification
                        {
                            UserId = ua.UserId,
                            AssignmentId = assignment.Id,
                            Title = subject,
                            Body = $"Recordatorio enviado para {cycleStartDate}",
                            SentAt = DateTime.UtcNow,
                            IsSent = true,
                            Type = notificationType
                        };

                        _context.Notifications.Add(notification);
                        _logger.LogInformation($"[Mail Enviado] Tarea: {assignment.Name}, Ciclo: {cycle} -> {ua.User.Email}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"[Error] Fallo al enviar a {ua.User.Email}");
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}