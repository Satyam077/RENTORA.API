using MongoDB.Driver;
using RENTARA.API.Models;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Repository
{
    public class AgreementRepository : IAgreementRepository
    {
        private readonly MongoDbSettings _ctx;

        public AgreementRepository(MongoDbSettings ctx)
        {
            _ctx = ctx;
        }

        // Basic CRUD operations
        public async Task<IEnumerable<Agreement>> GetAllAsync()
        {
            return await _ctx.Agreements.Find(_ => true).ToListAsync();
        }

        public async Task<Agreement?> GetByIdAsync(string id)
        {
            return await _ctx.Agreements.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Agreement>> GetByOwnerIdAsync(string ownerId)
        {
            return await _ctx.Agreements.Find(a => a.OwnerId == ownerId).ToListAsync();
        }

        public async Task<IEnumerable<Agreement>> GetByPropertyIdAsync(string propertyId)
        {
            return await _ctx.Agreements.Find(a => a.PropertyId == propertyId).ToListAsync();
        }

        public async Task<IEnumerable<Agreement>> GetByUnitIdAsync(string unitId)
        {
            return await _ctx.Agreements.Find(a => a.UnitId == unitId).ToListAsync();
        }

        public async Task<IEnumerable<Agreement>> GetByTenantIdAsync(string tenantId)
        {
            return await _ctx.Agreements.Find(a => a.TenantId == tenantId).ToListAsync();
        }

        public async Task<Agreement> CreateAsync(Agreement agreement)
        {
            agreement.CreatedAt = DateTime.UtcNow;
            agreement.IsActive = true;
            agreement.IsDeleted = false;

            await _ctx.Agreements.InsertOneAsync(agreement);
            return agreement;
        }

        public async Task<Agreement?> UpdateAsync(Agreement agreement)
        {
            agreement.UpdatedAt = DateTime.UtcNow;

            var result = await _ctx.Agreements.ReplaceOneAsync(
                a => a.Id == agreement.Id,
                agreement
            );

            return result.ModifiedCount > 0 ? agreement : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _ctx.Agreements.DeleteOneAsync(a => a.Id == id);
            return result.DeletedCount > 0;
        }

        // Methods with joined data
        public async Task<IEnumerable<AgreementResponseDTO>> GetAllAgreementsWithDetailsAsync()
        {
            var agreements = await _ctx.Agreements.Find(_ => true).ToListAsync();
            return await JoinAgreementWithDetails(agreements);
        }

        public async Task<AgreementResponseDTO?> GetAgreementWithDetailsByIdAsync(string id)
        {
            var agreement = await _ctx.Agreements.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (agreement == null) return null;

            var agreements = new List<Agreement> { agreement };
            var result = await JoinAgreementWithDetails(agreements);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<AgreementResponseDTO>> GetAgreementsByOwnerIdWithDetailsAsync(string ownerId)
        {
            var agreements = await _ctx.Agreements.Find(a => a.OwnerId == ownerId).ToListAsync();
            return await JoinAgreementWithDetails(agreements);
        }

        public async Task<IEnumerable<AgreementResponseDTO>> GetAgreementsByPropertyIdWithDetailsAsync(string propertyId)
        {
            var agreements = await _ctx.Agreements.Find(a => a.PropertyId == propertyId).ToListAsync();
            return await JoinAgreementWithDetails(agreements);
        }

        public async Task<IEnumerable<AgreementResponseDTO>> GetAgreementsByUnitIdWithDetailsAsync(string unitId)
        {
            var agreements = await _ctx.Agreements.Find(a => a.UnitId == unitId).ToListAsync();
            return await JoinAgreementWithDetails(agreements);
        }

        public async Task<IEnumerable<AgreementResponseDTO>> GetAgreementsByTenantIdWithDetailsAsync(string tenantId)
        {
            var agreements = await _ctx.Agreements.Find(a => a.TenantId == tenantId).ToListAsync();
            return await JoinAgreementWithDetails(agreements);
        }

        /// <summary>
        /// Helper method to join Agreement data with Property, Unit, and Tenant data
        /// </summary>
        private async Task<List<AgreementResponseDTO>> JoinAgreementWithDetails(List<Agreement> agreements)
        {
            var result = new List<AgreementResponseDTO>();

            foreach (var agreement in agreements)
            {
                // Fetch property data
                var property = await _ctx.Properties.Find(p => p.Id == agreement.PropertyId).FirstOrDefaultAsync();

                // Fetch unit data
                var unit = await _ctx.Units.Find(u => u.Id == agreement.UnitId).FirstOrDefaultAsync();

                // Fetch tenant data
                var tenant = await _ctx.Tenants.Find(t => t.Id == agreement.TenantId).FirstOrDefaultAsync();
                
                // Fetch user data for tenant name
                string tenantName = "Unknown Tenant";
                if (tenant != null)
                {
                    var user = await _ctx.Users.Find(u => u.Id == tenant.UserId).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        tenantName = user.FullName;
                    }
                }

                result.Add(new AgreementResponseDTO
                {
                    Id = agreement.Id,
                    PropertyId = agreement.PropertyId,
                    PropertyName = property?.PropertyName ?? "Unknown Property",
                    UnitId = agreement.UnitId,
                    UnitName = unit?.UnitName ?? "Unknown Unit",
                    TenantId = agreement.TenantId,
                    TenantName = tenantName,
                    OwnerId = agreement.OwnerId,
                    AgreementNumber = agreement.AgreementNumber,
                    AgreementType = agreement.AgreementType,
                    StartDate = agreement.StartDate,
                    EndDate = agreement.EndDate,
                    RentAmount = agreement.RentAmount,
                    SecurityDeposit = agreement.SecurityDeposit,
                    RentDueDay = agreement.RentDueDay,
                    AgreementFileUrl = agreement.AgreementFileUrl ?? string.Empty,
                    Status = agreement.Status,
                    TerminatedOn = agreement.TerminatedOn,
                    TerminationReason = agreement.TerminationReason ?? string.Empty,
                    IsRenewed = agreement.IsRenewed,
                    RenewedFromAgreementId = agreement.RenewedFromAgreementId ?? string.Empty,
                    Notes = agreement.Notes ?? string.Empty,
                    IsActive = agreement.IsActive,
                    CreatedAt = agreement.CreatedAt,
                    UpdatedAt = agreement.UpdatedAt,
                    CreatedBy = agreement.CreatedBy,
                    UpdatedBy = agreement.UpdatedBy
                });
            }

            return result;
        }
    }
}
