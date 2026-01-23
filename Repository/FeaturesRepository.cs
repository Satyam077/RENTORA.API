using MongoDB.Driver;
using RENTORA.API.Models;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Repository
{
    public class FeaturesRepository : IFeaturesRepository
    {
        private readonly MongoDbSettings _ctx;

        public FeaturesRepository(MongoDbSettings ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Features>> GetAllAsync()
        {
            return await _ctx.Features.Find(_ => true).ToListAsync();
        }

        public async Task<Features?> GetByIdAsync(string id)
        {
            return await _ctx.Features.Find(f => f.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Features> CreateAsync(Features feature)
        {
             feature.CreatedAt = DateTime.UtcNow;
             feature.IsActive = true; 
             await _ctx.Features.InsertOneAsync(feature);
             return feature;
        }

        public async Task<Features?> UpdateAsync(string id, Features feature)
        {
            feature.UpdatedAt = DateTime.UtcNow;
            
            var result = await _ctx.Features.ReplaceOneAsync(f => f.Id == id, feature);
            
            if (result.ModifiedCount == 0) return null;
            return feature;
        }

        public async Task<bool> DeleteAsync(string id)
        {
             var result = await _ctx.Features.DeleteOneAsync(f => f.Id == id);
             return result.DeletedCount > 0;
        }
    }
}
