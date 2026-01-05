using RENTORA.API.Models;

namespace RENTORA.API.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<Registration?> GetUserByEmailAsync(string email);
        Task<Registration?> GetUserByMobileAsync(string mobile);
        Task<Registration?> GetUserByEmailOrMobileAsync(string emailOrMobile);
        Task<Registration?> GetUserByIdAsync(string id);
        Task<Registration> CreateUserAsync(Registration user);
        Task<bool> UpdateUserAsync(Registration user);
        Task<bool> DeleteUserAsync(string id);
        Task<IEnumerable<Registration>> GetAllUsersAsync();
        Task<bool> UpdatePasswordAsync(string userId, string passwordHash, string passwordSalt);
    }
}
