using MongoDB.Driver;
using RENTARA.API.Models;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Repository
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly MongoDbSettings _ctx;

        public PropertyRepository(MongoDbSettings ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<PropertyModel>> GetAllAsync()
        {
            return await _ctx.Properties.Find(_ => true).ToListAsync();
        }

        public async Task<PropertyModel?> GetByIdAsync(string id)
        {
            return await _ctx.Properties.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PropertyModel>> GetByOwnerIdAsync(string ownerId)
        {
            return await _ctx.Properties.Find(p => p.OwnerId == ownerId).ToListAsync();
        }

        public async Task<PropertyModel> CreateAsync(PropertyModel property)
        {
            property.CreatedAt = DateTime.UtcNow;
            property.CreatedBy = "System";
            property.IsActive = true;
            
            // Calculate total and occupied units
            property.TotalUnits = property.Units?.Count ?? 0;
            property.OccupiedUnits = property.Units?.Count(u => u.IsOccupied) ?? 0;
            property.IsFullyOccupied = property.TotalUnits > 0 && property.OccupiedUnits == property.TotalUnits;

            await _ctx.Properties.InsertOneAsync(property);
            return property;
        }

        public async Task<PropertyModel?> UpdateAsync(PropertyModel property)
        {
            property.UpdatedAt = DateTime.UtcNow;
            property.UpdatedBy = "System";
            
            // Recalculate occupancy
            property.TotalUnits = property.Units?.Count ?? 0;
            property.OccupiedUnits = property.Units?.Count(u => u.IsOccupied) ?? 0;
            property.IsFullyOccupied = property.TotalUnits > 0 && property.OccupiedUnits == property.TotalUnits;

            var result = await _ctx.Properties.ReplaceOneAsync(
                p => p.Id == property.Id,
                property
            );

            return result.ModifiedCount > 0 ? property : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _ctx.Properties.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ExistsAsync(string propertyName, string ownerId)
        {
            var count = await _ctx.Properties.CountDocumentsAsync(
                p => p.PropertyName.ToLower() == propertyName.ToLower() && p.OwnerId == ownerId
            );
            return count > 0;
        }
    }
}
