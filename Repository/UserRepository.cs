using MongoDB.Driver;
using RENTORA.API.Models;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Repository
{
    public class UserRepository : IUserRepository
    {
        //private readonly IMongoCollection<Registration> _users;
        private readonly MongoDbSettings _ctx;

        public UserRepository(MongoDbSettings ctx)
        {
            _ctx = ctx;
        }

        public async Task<Registration?> GetUserByEmailAsync(string email)
        {
            return await _ctx.Users.Find(u => u.Email == email && !u.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Registration?> GetUserByMobileAsync(string mobile)
        {
            return await _ctx.Users.Find(u => u.Mobile == mobile && !u.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Registration?> GetUserByEmailOrMobileAsync(string emailOrMobile)
        {
            return await _ctx.Users.Find(u => 
                (u.Email == emailOrMobile || u.Mobile == emailOrMobile) && !u.IsDeleted
            ).FirstOrDefaultAsync();
        }

        public async Task<Registration?> GetUserByIdAsync(string id)
        {
            return await _ctx.Users.Find(u => u.Id == id && !u.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Registration> CreateUserAsync(Registration user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.IsActive = true;
            user.IsDeleted = false;
            
            await _ctx.Users.InsertOneAsync(user);
            return user;
        }

        public async Task<bool> UpdateUserAsync(Registration user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _ctx.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var update = Builders<Registration>.Update
                .Set(u => u.IsDeleted, true)
                .Set(u => u.UpdatedAt, DateTime.UtcNow);
            
            var result = await _ctx.Users.UpdateOneAsync(u => u.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<IEnumerable<Registration>> GetAllUsersAsync()
        {
            return await _ctx.Users.Find(u => !u.IsDeleted).ToListAsync();
        }
    }
}
