using MongoDB.Driver;
using RENTORA.API.Models;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Repository
{
    public class UnitsRepository : IUnitsRepository
    {
        private readonly MongoDbSettings _ctx;

        public UnitsRepository(MongoDbSettings ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<UnitModel>> GetAllAsync()
        {
            return await _ctx.Units.Find(_ => true).ToListAsync();
        }

        public async Task<UnitModel?> GetByIdAsync(string id)
        {
            return await _ctx.Units.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UnitModel>> GetByPropertyIdAsync(string propertyId)
        {
            return await _ctx.Units.Find(u => u.PropertyId == propertyId).ToListAsync();
        }

        public async Task<IEnumerable<UnitModel>> GetByOwnerIdAsync(string ownerId)
        {
            return await _ctx.Units.Find(u => u.OwnerId == ownerId).ToListAsync();
        }

        public async Task<UnitModel> CreateAsync(UnitModel unit)
        {
            unit.CreatedAt = DateTime.UtcNow;
            unit.UpdatedAt = DateTime.UtcNow;
            unit.IsActive = true;
            unit.IsDeleted = false;
            
            await _ctx.Units.InsertOneAsync(unit);
            return unit;
        }

        public async Task<UnitModel?> UpdateAsync(UnitModel unit)
        {
            unit.UpdatedAt = DateTime.UtcNow;

            var result = await _ctx.Units.ReplaceOneAsync(
                u => u.Id == unit.Id,
                unit
            );

            return result.ModifiedCount > 0 ? unit : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _ctx.Units.DeleteOneAsync(u => u.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsAsync(string unitName, string propertyId)
        {
            var count = await _ctx.Units.CountDocumentsAsync(
                u => u.UnitName.ToLower() == unitName.ToLower() && u.PropertyId == propertyId
            );
            return count > 0;
        }
    }
}
