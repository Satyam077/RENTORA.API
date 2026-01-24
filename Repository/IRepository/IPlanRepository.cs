using RENTORA.API.Models;

namespace RENTORA.API.Repository.IRepository
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plans>> GetAllAsync();
        Task<Plans?> GetByIdAsync(string id);
        Task<Plans> UpsertAsync(Plans plan, List<string> featureIds);
        Task DeleteAsync(string id);
    }
}
