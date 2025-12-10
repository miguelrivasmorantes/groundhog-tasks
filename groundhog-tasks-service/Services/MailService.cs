using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace groundhog_tasks_service.Services
{
    public class MailService : IMailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public MailService(IConfiguration config)
        {
            var host = config["SMTP_HOST"] ?? "smtp.gmail.com";

            int port = int.TryParse(config["SMTP_PORT"], out int p) ? p : 587;

            _senderEmail = config["SMTP_EMAIL"] ??
                           throw new ArgumentNullException("SMTP_EMAIL", "Falta el correo en .env");

            var password = config["SMTP_PASSWORD"] ??
                           throw new ArgumentNullException("SMTP_PASSWORD", "Falta la contraseña de aplicación en .env");

            _senderName = config["SMTP_SENDER_NAME"] ?? "System";

            _smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_senderEmail, password)
            };
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string htmlBody)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_senderEmail, _senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(recipientEmail);

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fallo SMTP al enviar a {recipientEmail}: {ex.Message}");
            }
        }
    }
}