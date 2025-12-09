using MongoDB.Driver;
using RENTARA.API.Models;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Repository
{
    public class TenantsRepository : ITenantsRepository
    {
        private readonly MongoDbSettings _ctx;

        public TenantsRepository(MongoDbSettings ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _ctx.Tenants.Find(_ => true).ToListAsync();
        }

        public async Task<Tenant?> GetByIdAsync(string id)
        {
            return await _ctx.Tenants.Find(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Tenant>> GetByOwnerIdAsync(string ownerId)
        {
            return await _ctx.Tenants.Find(t => t.OwnerId == ownerId).ToListAsync();
        }

        public async Task<IEnumerable<Tenant>> GetByPropertyIdAsync(string propertyId)
        {
            return await _ctx.Tenants.Find(t => t.PropertyId == propertyId).ToListAsync();
        }

        public async Task<IEnumerable<Tenant>> GetByUnitIdAsync(string unitId)
        {
            return await _ctx.Tenants.Find(t => t.UnitId == unitId).ToListAsync();
        }

        public async Task<Tenant> CreateAsync(Tenant tenant)
        {
            tenant.CreatedAt = DateTime.UtcNow;
            tenant.UpdatedAt = DateTime.UtcNow;
            tenant.IsActive = true;
            tenant.IsDeleted = false;
            tenant.IsActiveTenant = true;
            tenant.IsMovedOut = false;

            await _ctx.Tenants.InsertOneAsync(tenant);
            return tenant;
        }

        public async Task<Tenant?> UpdateAsync(Tenant tenant)
        {
            tenant.UpdatedAt = DateTime.UtcNow;

            var result = await _ctx.Tenants.ReplaceOneAsync(
                t => t.Id == tenant.Id,
                tenant
            );

            return result.ModifiedCount > 0 ? tenant : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _ctx.Tenants.DeleteOneAsync(t => t.Id == id);
            return result.DeletedCount > 0;
        }

        // New methods with joined User data
        public async Task<IEnumerable<TenantResponseDTO>> GetAllTenantsWithUserDataAsync()
        {
            var tenants = await _ctx.Tenants.Find(_ => true).ToListAsync();
            return await JoinTenantWithUserData(tenants);
        }

        public async Task<TenantResponseDTO?> GetTenantWithUserDataByIdAsync(string id)
        {
            var tenant = await _ctx.Tenants.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (tenant == null) return null;

            var tenants = new List<Tenant> { tenant };
            var result = await JoinTenantWithUserData(tenants);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<TenantResponseDTO>> GetTenantsByOwnerIdWithUserDataAsync(string ownerId)
        {
            var tenants = await _ctx.Tenants.Find(t => t.OwnerId == ownerId).ToListAsync();
            return await JoinTenantWithUserData(tenants);
        }

        public async Task<IEnumerable<TenantResponseDTO>> GetTenantsByPropertyIdWithUserDataAsync(string propertyId)
        {
            var tenants = await _ctx.Tenants.Find(t => t.PropertyId == propertyId).ToListAsync();
            return await JoinTenantWithUserData(tenants);
        }

        public async Task<IEnumerable<TenantResponseDTO>> GetTenantsByUnitIdWithUserDataAsync(string unitId)
        {
            var tenants = await _ctx.Tenants.Find(t => t.UnitId == unitId).ToListAsync();
            return await JoinTenantWithUserData(tenants);
        }

        /// <summary>
        /// Helper method to join Tenant data with User (Registration) data
        /// Eliminates redundancy by fetching user details from Users collection
        /// </summary>
        private async Task<List<TenantResponseDTO>> JoinTenantWithUserData(List<Tenant> tenants)
        {
            var result = new List<TenantResponseDTO>();

            foreach (var tenant in tenants)
            {
                // Fetch user data from Users collection
                var user = await _ctx.Users.Find(u => u.Id == tenant.UserId).FirstOrDefaultAsync();

                if (user != null)
                {
                    result.Add(new TenantResponseDTO
                    {
                        // Tenant ID
                        Id = tenant.Id,

                        // User Information (from Registration/Users collection)
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Mobile = user.Mobile,
                        Gender = user.Gender ?? string.Empty,
                        DateOfBirth = user.DateOfBirth,
                        IsEmailVerified = user.IsEmailVerified,
                        IsMobileVerified = user.IsMobileVerified,
                        ProfileImageUrl = user.ProfileImageUrl ?? string.Empty,

                        // Tenant-specific Information (from Tenants collection)
                        OwnerId = tenant.OwnerId,
                        PropertyId = tenant.PropertyId,
                        UnitId = tenant.UnitId,
                        PermanentAddress = tenant.PermanentAddress,
                        CurrentAddress = tenant.CurrentAddress,

                        // Rent & Agreement Info
                        RentAmount = tenant.RentAmount,
                        SecurityDeposit = tenant.SecurityDeposit,
                        RentDueDay = tenant.RentDueDay,
                        AgreementStartDate = tenant.AgreementStartDate,
                        AgreementEndDate = tenant.AgreementEndDate,
                        IsAgreementExpired = tenant.IsAgreementExpired,

                        // KYC / ID Proof
                        Documents = tenant.Documents,
                        IdProofType = tenant.IdProofType ?? string.Empty,
                        IdProofNumber = tenant.IdProofNumber ?? string.Empty,

                        // Status Tracking
                        IsActiveTenant = tenant.IsActiveTenant,
                        IsRentPending = tenant.IsRentPending,
                        IsMovedOut = tenant.IsMovedOut,
                        MoveInDate = tenant.MoveInDate,
                        MoveOutDate = tenant.MoveOutDate,
                        Notes = tenant.Notes,

                        // Base Entity Fields
                        IsActive = tenant.IsActive,
                        CreatedAt = tenant.CreatedAt,
                        UpdatedAt = tenant.UpdatedAt,
                        CreatedBy = tenant.CreatedBy,
                        UpdatedBy = tenant.UpdatedBy
                    });
                }
            }

            return result;
        }
    }
}
