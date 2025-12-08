using MongoDB.Driver;
using RENTARA.API.Models;
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

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            var count = await _ctx.Tenants.CountDocumentsAsync(
                t => t.Email.ToLower() == email.ToLower()
            );
            return count > 0;
        }

        public async Task<bool> ExistsByMobileAsync(string mobile)
        {
            var count = await _ctx.Tenants.CountDocumentsAsync(
                t => t.Mobile == mobile
            );
            return count > 0;
        }
    }
}
