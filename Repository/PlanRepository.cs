using MongoDB.Driver;
using RENTORA.API.Models;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Repository
{
    public class PlanRepository : IPlanRepository
    {
        private readonly MongoDbSettings _ctx;

        public PlanRepository(MongoDbSettings ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<Plans>> GetAllAsync()
        {
            return await _ctx.Plans.Find(_ => true).ToListAsync();
        }

        public async Task<Plans?> GetByIdAsync(string id)
        {
            return await _ctx.Plans.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Plans> UpsertAsync(Plans plan, List<string> featureIds)
        {
            // Fetch selected features
            var featuresList = new List<Features>();
            if (featureIds != null && featureIds.Any())
            {
                var filter = Builders<Features>.Filter.In(f => f.Id, featureIds);
                featuresList = await _ctx.Features.Find(filter).ToListAsync();
            }

            plan.Features = featuresList;

            if (string.IsNullOrEmpty(plan.Id))
            {
                // Create
                plan.CreatedAt = DateTime.UtcNow;
                plan.IsActive = true;
                await _ctx.Plans.InsertOneAsync(plan);
            }
            else
            {
                // Update
                plan.UpdatedAt = DateTime.UtcNow;
                // Ensure we don't overwrite CreatedAt if we didn't fetch it first?
                // Ideally we should fetch existing first to keep audit fields if not passed in plan object.
                // The controller creates a new Plans object from DTO.
                // So we should fetch existing one to get CreatedAt etc.
                
                var existingPlan = await _ctx.Plans.Find(p => p.Id == plan.Id).FirstOrDefaultAsync();
                if (existingPlan != null)
                {
                    plan.CreatedAt = existingPlan.CreatedAt;
                    plan.IsActive = existingPlan.IsActive; // Maintain status
                    
                    await _ctx.Plans.ReplaceOneAsync(p => p.Id == plan.Id, plan);
                }
                else
                {
                    // ID was passed but not found. Allow create? Or error?
                    // Let's treat as Create if ID is invalid? No, usually not.
                    // But if it's Upsert, maybe custom ID? MongoDB usually auto-generates if empty.
                    // If ID provided, typically we expect it to exist or we force it.
                    // Let's assume Update flow if ID is present.
                    // If not found, return null to indicate failure to update.
                    return null;
                }
            }

            return plan;
        }

        public async Task DeleteAsync(string id)
        {
            await _ctx.Plans.DeleteOneAsync(p => p.Id == id);
        }
    }
}
