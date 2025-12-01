using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RENTORA.API.Services.IServices;
using RENTORA.API.WebSettings;
using MailKit.Net.Smtp;

namespace RENTORA.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage,
                TextBody = StripHtml(htmlMessage)
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.SmtpHost,
                _settings.SmtpPort,
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);


            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private string StripHtml(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
