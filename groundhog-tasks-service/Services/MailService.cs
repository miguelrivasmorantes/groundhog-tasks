using Resend;
using Microsoft.Extensions.Configuration;

namespace groundhog_tasks_service.Services
{
    public class MailService : IMailService
    {
        private readonly IResend _client;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public MailService(IConfiguration config)
        {
            var apiKey = config["RESEND_API_KEY"] ??
                         throw new ArgumentNullException("RESEND_API_KEY", "La clave API de Resend no se encontró.");

            _senderEmail = config["RESEND_SENDER_EMAIL"] ?? "default@example.com";
            _senderName = config["RESEND_SENDER_NAME"] ?? "System";

            _client = ResendClient.Create(apiKey);
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string htmlBody)
        {
            var email = new EmailMessage
            {
                From = $"{_senderName} <{_senderEmail}>",
                To = new[] { recipientEmail },
                Subject = subject,
                HtmlBody = htmlBody,
                TextBody = "Por favor, habilita HTML para ver este mensaje."
            };

            var response = await _client.EmailSendAsync(email);

            if (response.Content == Guid.Empty)
            {
                throw new Exception("El envío de Resend falló y no devolvió un ID válido.");
            }
        }
    }
}