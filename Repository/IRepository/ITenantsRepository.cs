using RENTARA.API.Models;

namespace RENTORA.API.Repository.IRepository
{
    public interface ITenantsRepository
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant?> GetByIdAsync(string id);
        Task<IEnumerable<Tenant>> GetByOwnerIdAsync(string ownerId);
        Task<IEnumerable<Tenant>> GetByPropertyIdAsync(string propertyId);
        Task<IEnumerable<Tenant>> GetByUnitIdAsync(string unitId);
        Task<Tenant> CreateAsync(Tenant tenant);
        Task<Tenant?> UpdateAsync(Tenant tenant);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByMobileAsync(string mobile);
    }
}
