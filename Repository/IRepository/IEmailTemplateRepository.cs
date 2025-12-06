using RENTORA.API.Models;
using RENTORA.API.Models.Enums;

namespace RENTORA.API.Repository.IRepository
{
    public interface IEmailTemplateRepository
    {
        Task<IEnumerable<EmailTemplate>> GetAllAsync();
        Task<EmailTemplate?> GetByIdAsync(string id);
        Task<EmailTemplate> CreateAsync(EmailTemplate emailTemplate);
        Task<IEnumerable<EmailTemplate>> GetByApplicableForAsync(string applicableFor);
        Task<EmailTemplate?> UpdateAsync(EmailTemplate emailTemplate);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string templateName);
        Task<EmailTemplate?> GetByTemplateNameAsync(EmailTemplateName template);
    }
}
