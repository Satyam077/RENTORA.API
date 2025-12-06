using RENTORA.API.Models.Enums;

namespace RENTORA.API.Services.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendTemplateEmailAsync(string toEmail,string toName, EmailTemplateName templateName, Dictionary<string, string> tokens);

    }
}
