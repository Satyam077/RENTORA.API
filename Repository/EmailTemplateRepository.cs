using MongoDB.Driver;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.WebSettings;

namespace RENTORA.API.Repository
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly MongoDbSettings _ctx;

        public EmailTemplateRepository(MongoDbSettings ctx)
        {
            _ctx = ctx;
        }

        public async Task<EmailTemplate> CreateAsync(EmailTemplate emailTemplate)
        {
            //emailTemplate.Id = Guid.NewGuid().ToString();
            emailTemplate.CreatedAt = DateTime.UtcNow;
            emailTemplate.IsActive = true;
            emailTemplate.IsDeleted = false;

            await _ctx.EmailTemplates.InsertOneAsync(emailTemplate);
            return emailTemplate;
        }

        public async Task<EmailTemplate?> GetByIdAsync(string id)
        {
            return await _ctx.EmailTemplates
                .Find(e => e.Id == id && !e.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EmailTemplate>> GetAllAsync()
        {
            return await _ctx.EmailTemplates
                .Find(e => !e.IsDeleted)
                .SortByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmailTemplate>> GetByApplicableForAsync(string applicableFor)
        {
            return await _ctx.EmailTemplates
                .Find(e => e.ApplicableFor == applicableFor && !e.IsDeleted)
                .SortByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<EmailTemplate?> UpdateAsync(EmailTemplate emailTemplate)
        {
            var existing = await _ctx.EmailTemplates
                .Find(e => e.Id == emailTemplate.Id && !e.IsDeleted)
                .FirstOrDefaultAsync();

            if (existing == null)
                return null;

            existing.TemplateName = emailTemplate.TemplateName;
            existing.EmailSubject = emailTemplate.EmailSubject;
            existing.EmailBody = emailTemplate.EmailBody;
            existing.Tokens = emailTemplate.Tokens;
            existing.ApplicableFor = emailTemplate.ApplicableFor;
            existing.IsActive = emailTemplate.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _ctx.EmailTemplates.ReplaceOneAsync(
                e => e.Id == emailTemplate.Id,
                existing
            );

            return result.ModifiedCount > 0 ? existing : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var update = Builders<EmailTemplate>.Update
                .Set(e => e.IsDeleted, true)
                .Set(e => e.UpdatedAt, DateTime.UtcNow);

            var result = await _ctx.EmailTemplates.UpdateOneAsync(
                e => e.Id == id && !e.IsDeleted,
                update
            );

            return result.ModifiedCount > 0;
        }

        public async Task<bool> ExistsAsync(string templateName)
        {
            var count = await _ctx.EmailTemplates
                .CountDocumentsAsync(e => e.TemplateName == templateName && !e.IsDeleted);

            return count > 0;
        }
        public async Task<EmailTemplate?> GetByTemplateNameAsync(EmailTemplateName templateName)
        {
            string displayName = Helper.GetDisplayName(templateName).Trim();
            return await _ctx.EmailTemplates
                .Find(e => e.TemplateName.Trim() == displayName && !e.IsDeleted)
                .FirstOrDefaultAsync();
        }
    }
}
