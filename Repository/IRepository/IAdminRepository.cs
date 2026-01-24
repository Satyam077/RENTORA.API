namespace RENTORA.API.Repository.IRepository
{
    public interface IAdminRepository
    {
        Task<long> GetTotalLandlordsAsync();
        Task<long> GetTotalAdminsAsync();
        Task<long> GetTotalTenantsAsync();
        Task<decimal> GetTotalRevenueAsync();
    }
}
