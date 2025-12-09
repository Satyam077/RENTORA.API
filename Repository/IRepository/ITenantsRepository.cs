using RENTARA.API.Models;
using RENTORA.API.Models.DTOs;

namespace RENTORA.API.Repository.IRepository
{
    public interface ITenantsRepository
    {
        // Original methods (still needed for internal operations)
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant?> GetByIdAsync(string id);
        Task<IEnumerable<Tenant>> GetByOwnerIdAsync(string ownerId);
        Task<IEnumerable<Tenant>> GetByPropertyIdAsync(string propertyId);
        Task<IEnumerable<Tenant>> GetByUnitIdAsync(string unitId);
        Task<Tenant> CreateAsync(Tenant tenant);
        Task<Tenant?> UpdateAsync(Tenant tenant);
        Task<bool> DeleteAsync(string id);

        // New methods with joined User data (no redundancy)
        Task<IEnumerable<TenantResponseDTO>> GetAllTenantsWithUserDataAsync();
        Task<TenantResponseDTO?> GetTenantWithUserDataByIdAsync(string id);
        Task<IEnumerable<TenantResponseDTO>> GetTenantsByOwnerIdWithUserDataAsync(string ownerId);
        Task<IEnumerable<TenantResponseDTO>> GetTenantsByPropertyIdWithUserDataAsync(string propertyId);
        Task<IEnumerable<TenantResponseDTO>> GetTenantsByUnitIdWithUserDataAsync(string unitId);
    }
}
