using RENTORA.API.Models;

namespace RENTORA.API.Repository.IRepository
{
    public interface IFeaturesRepository
    {
        Task<IEnumerable<Features>> GetAllAsync();
        Task<Features?> GetByIdAsync(string id);
        Task<Features> CreateAsync(Features feature);
        Task<Features?> UpdateAsync(string id, Features feature);
        Task<bool> DeleteAsync(string id);
    }
}
