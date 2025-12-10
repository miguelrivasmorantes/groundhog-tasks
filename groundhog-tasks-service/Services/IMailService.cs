namespace groundhog_tasks_service.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string recipientEmail, string subject, string htmlBody);
    }
}