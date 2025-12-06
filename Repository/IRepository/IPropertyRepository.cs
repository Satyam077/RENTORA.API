using RENTARA.API.Models;

namespace RENTORA.API.Repository.IRepository
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<PropertyModel>> GetAllAsync();
        Task<PropertyModel?> GetByIdAsync(string id);
        Task<IEnumerable<PropertyModel>> GetByOwnerIdAsync(string ownerId);
        Task<PropertyModel> CreateAsync(PropertyModel property);
        Task<PropertyModel?> UpdateAsync(PropertyModel property);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string propertyName, string ownerId);
    }
}
