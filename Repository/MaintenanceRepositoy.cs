using MongoDB.Driver;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;
using RENTARA.API.Models;

namespace RENTORA.API.Repository
{
    public class MaintenanceRepositoy : IMaintenanceRepositoy
    {
        private readonly IMongoCollection<Maintenance> _maintenance;
        private readonly IMongoCollection<PropertyModel> _properties;
        private readonly IMongoCollection<UnitModel> _unit;

        public MaintenanceRepositoy(MongoDbSettings dbSettings)
        {
            _maintenance = dbSettings.Maintenance;
            _properties = dbSettings.Properties;
            _unit = dbSettings.Units;
        }

        public async Task<IEnumerable<Maintenance>> GetAllAsync()
        {
            return await _maintenance.Find(_ => true).ToListAsync();
        }

        public async Task<Maintenance?> GetByIdAsync(string id)
        {
            return await _maintenance.Find(m => m.Id == id && !m.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Maintenance>> GetByTenantIdAsync(string tenantId)
        {
            return await _maintenance.Find(m => m.TenantId == tenantId && !m.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Maintenance>> GetByPropertyIdAsync(string propertyId)
        {
            return await _maintenance.Find(m => m.PropertyId == propertyId && !m.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Maintenance>> GetByUnitIdAsync(string unitId)
        {
            return await _maintenance.Find(m => m.UnitId == unitId && !m.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Maintenance>> GetByOwnerIdAsync(string ownerId)
        {
            // Direct query by OwnerId - much faster than joining with properties
            return await _maintenance.Find(m => m.OwnerId == ownerId && !m.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Maintenance>> GetByLandlordIdAsync(string landlordId)
        {
            // Step 1: Get all maintenance requests for this landlord
            var maintenances = await _maintenance
                .Find(m => m.OwnerId == landlordId && !m.IsDeleted)
                .ToListAsync();

            if (!maintenances.Any())
                return maintenances;

            // Step 2: Get unique property IDs from maintenance requests
            var propertyIds = maintenances
                .Select(m => m.PropertyId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            // Step 3: Load properties
            var properties = await _properties
                .Find(p => propertyIds.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync();

            // Step 4: Get unique unit IDs from maintenance requests
            var unitIds = maintenances
                .Select(m => m.UnitId)
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            // Step 5: Load units
            var units = await _unit
                .Find(u => unitIds.Contains(u.Id) && !u.IsDeleted)
                .ToListAsync();

            // Step 6: Populate navigation properties
            foreach (var maintenance in maintenances)
            {
                // Attach property
                if (!string.IsNullOrEmpty(maintenance.PropertyId))
                {
                    maintenance.Property = properties.FirstOrDefault(p => p.Id == maintenance.PropertyId);
                }
                
                // Attach unit
                if (!string.IsNullOrEmpty(maintenance.UnitId))
                {
                    maintenance.Unit = units.FirstOrDefault(u => u.Id == maintenance.UnitId);
                }
            }

            return maintenances;
        }


        public async Task<Maintenance> CreateAsync(Maintenance maintenance)
        {
            maintenance.Status = Status.Open;
            maintenance.CreatedAt = DateTime.UtcNow;
            maintenance.IsActive = true;
            maintenance.IsDeleted = false;
            await _maintenance.InsertOneAsync(maintenance);
            return maintenance;
        }

        public async Task<Maintenance?> UpdateAsync(Maintenance maintenance)
        {
            maintenance.UpdatedAt = DateTime.UtcNow;
            maintenance.UpdateCount++;
            
            var result = await _maintenance.ReplaceOneAsync(
                m => m.Id == maintenance.Id && !m.IsDeleted, 
                maintenance
            );
            
            return result.ModifiedCount > 0 ? maintenance : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var update = Builders<Maintenance>.Update
                .Set(m => m.IsDeleted, true)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);
            
            var result = await _maintenance.UpdateOneAsync(
                m => m.Id == id && !m.IsDeleted, 
                update
            );
            
            return result.ModifiedCount > 0;
        }

        public async Task<Maintenance?> UpdateStatusAsync(string id, Status status)
        {
            var update = Builders<Maintenance>.Update
                .Set(m => m.Status, status)
                .Set(m => m.UpdatedAt, DateTime.UtcNow)
                .Inc(m => m.UpdateCount, 1);
            
            var result = await _maintenance.FindOneAndUpdateAsync(
                m => m.Id == id && !m.IsDeleted,
                update,
                new FindOneAndUpdateOptions<Maintenance> { ReturnDocument = ReturnDocument.After }
            );
            
            return result;
        }

        public async Task<Maintenance?> ScheduleMaintenanceAsync(string id, DateTime scheduledDate)
        {
            var update = Builders<Maintenance>.Update
                .Set(m => m.ScheduledDate, scheduledDate)
                .Set(m => m.Status, Status.Scheduled)
                .Set(m => m.UpdatedAt, DateTime.UtcNow)
                .Inc(m => m.UpdateCount, 1);
            
            var result = await _maintenance.FindOneAndUpdateAsync(
                m => m.Id == id && !m.IsDeleted,
                update,
                new FindOneAndUpdateOptions<Maintenance> { ReturnDocument = ReturnDocument.After }
            );
            
            return result;
        }

        public async Task<Maintenance?> RateMaintenanceAsync(string id, double rating)
        {
            var update = Builders<Maintenance>.Update
                .Set(m => m.Rating, rating)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);
            
            var result = await _maintenance.FindOneAndUpdateAsync(
                m => m.Id == id && !m.IsDeleted,
                update,
                new FindOneAndUpdateOptions<Maintenance> { ReturnDocument = ReturnDocument.After }
            );
            
            return result;
        }
    }
}

