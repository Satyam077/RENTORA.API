using MongoDB.Driver;
using RENTARA.API.Models;
using RENTORA.API.Models;
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
            var properties = await _ctx.Properties.Find(_ => true).ToListAsync();
            
            // Calculate unit counts for each property
            foreach (var property in properties)
            {
                await CalculateUnitCountsAsync(property);
            }
            
            return properties;
        }

        public async Task<PropertyModel?> GetByIdAsync(string id)
        {
            var property = await _ctx.Properties.Find(p => p.Id == id).FirstOrDefaultAsync();
            
            if (property != null)
            {
                await CalculateUnitCountsAsync(property);
            }
            
            return property;
        }

        public async Task<IEnumerable<PropertyModel>> GetByOwnerIdAsync(string ownerId)
        {
            var properties = await _ctx.Properties.Find(p => p.OwnerId == ownerId).ToListAsync();
            
            // Calculate unit counts for each property from Units collection
            foreach (var property in properties)
            {
                await CalculateUnitCountsAsync(property);
            }
            
            return properties;
        }

        /// <summary>
        /// Helper method to calculate TotalUnits and OccupiedUnits from the Units collection
        /// </summary>
        private async Task CalculateUnitCountsAsync(PropertyModel property)
        {
            if (property == null || string.IsNullOrEmpty(property.Id)) return;

            var units = await _ctx.Units.Find(u => u.PropertyId == property.Id).ToListAsync();
            
            property.TotalUnits = units.Count;
            property.OccupiedUnits = units.Count(u => u.IsOccupied);
            property.IsFullyOccupied = property.TotalUnits > 0 && property.OccupiedUnits == property.TotalUnits;
        }

        public async Task<PropertyModel> CreateAsync(PropertyModel property)
        {
            property.CreatedAt = DateTime.UtcNow;
            property.CreatedBy = "System";
            property.IsActive = true;
            
            // Initialize unit counts (will be 0 for new properties)
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
            
            // Recalculate occupancy from Units collection
            await CalculateUnitCountsAsync(property);

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
