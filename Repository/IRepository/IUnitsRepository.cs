using RENTORA.API.Models;

namespace RENTORA.API.Repository.IRepository
{
    public interface IUnitsRepository
    {
        Task<IEnumerable<UnitModel>> GetAllAsync();
        Task<UnitModel?> GetByIdAsync(string id);
        Task<IEnumerable<UnitModel>> GetByPropertyIdAsync(string propertyId);
        Task<IEnumerable<UnitModel>> GetByOwnerIdAsync(string ownerId);
        Task<UnitModel> CreateAsync(UnitModel unit);
        Task<UnitModel?> UpdateAsync(UnitModel unit);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string unitName, string propertyId);
    }
}
