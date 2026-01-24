using MongoDB.Driver;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IMongoCollection<Registration> _users;

        public AdminRepository(MongoDbSettings settings)
        {
            _users = settings.Users;
        }
        public async Task<long> GetTotalAdminsAsync()
        {
            return await _users.CountDocumentsAsync(u => u.Role == Role.Admin);
        }
        public async Task<long> GetTotalLandlordsAsync()
        {
            return await _users.CountDocumentsAsync(u => u.Role == Role.Landlords);
        }
        public async Task<long> GetTotalTenantsAsync()
        {
            return await _users.CountDocumentsAsync(u => u.Role == Role.Tenants);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            // Design only for now
            return await Task.FromResult(0m);
        }
    }
}
