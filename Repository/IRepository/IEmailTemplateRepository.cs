using RENTORA.API.Models;
using RENTORA.API.Models.Enums;

namespace RENTORA.API.Repository.IRepository
{
    public interface IEmailTemplateRepository
    {
        Task<EmailTemplate> CreateAsync(EmailTemplate emailTemplate);
        Task<EmailTemplate?> GetByIdAsync(string id);
        Task<IEnumerable<EmailTemplate>> GetAllAsync();
        Task<IEnumerable<EmailTemplate>> GetByApplicableForAsync(string applicableFor);
        Task<EmailTemplate?> UpdateAsync(EmailTemplate emailTemplate);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string templateName);
        Task<EmailTemplate?> GetByTemplateNameAsync(EmailTemplateName template);
    }
}
