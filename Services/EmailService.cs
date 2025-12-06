using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.Services.IServices;
using RENTORA.API.WebSettings;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RENTORA.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly SendGridClient _client;
        private readonly IEmailTemplateRepository _templateRepo;

        public EmailService(IOptions<EmailSettings> settings,
                            IEmailTemplateRepository templateRepo)
        {
            _settings = settings.Value;
            _client = new SendGridClient(_settings.ApiKey);
            _templateRepo = templateRepo;
        }

        public async Task SendTemplateEmailAsync(string toEmail,string toName,EmailTemplateName templateName, Dictionary<string, string> tokens)
        {
            // Get template from MongoDB
            var template = await _templateRepo.GetByTemplateNameAsync(templateName);

            if (template == null)
                throw new Exception($"Email template '{templateName}' not found.");

            // Replace tokens
            string body = ReplaceTokens(template.EmailBody, tokens);

            // Send final email
            await SendEmailAsync(toEmail, toName, template.EmailSubject, body);
        }

        private string ReplaceTokens(string template, Dictionary<string, string> tokens)
        {
            foreach (var token in tokens)
                template = template.Replace("{{" + token.Key + "}}", token.Value);

            return template;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlMessage)
        {
            var from = new EmailAddress(_settings.From, _settings.FromName);
            var to = new EmailAddress(toEmail,toName);

            var msg = MailHelper.CreateSingleEmail(
                from, to, subject,
                plainTextContent: StripHtml(htmlMessage),
                htmlContent: htmlMessage
            );

            var response = await _client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception("SendGrid Error: " + body);
            }
        }

        private string StripHtml(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
        }

        public Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
