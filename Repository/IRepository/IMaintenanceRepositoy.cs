using RENTORA.API.Models;
using RENTORA.API.Models.Enums;

namespace RENTORA.API.Repository.IRepository
{
    public interface IMaintenanceRepositoy
    {
        Task<IEnumerable<Maintenance>> GetAllAsync();
        Task<Maintenance?> GetByIdAsync(string id);
        Task<IEnumerable<Maintenance>> GetByTenantIdAsync(string tenantId);
        Task<IEnumerable<Maintenance>> GetByPropertyIdAsync(string propertyId);
        Task<IEnumerable<Maintenance>> GetByUnitIdAsync(string unitId);
        Task<IEnumerable<Maintenance>> GetByOwnerIdAsync(string ownerId);
        Task<IEnumerable<Maintenance>> GetByLandlordIdAsync(string landlordId);
        Task<Maintenance> CreateAsync(Maintenance maintenance);
        Task<Maintenance?> UpdateAsync(Maintenance maintenance);
        Task<bool> DeleteAsync(string id);
        Task<Maintenance?> UpdateStatusAsync(string id, Status status);
        Task<Maintenance?> ScheduleMaintenanceAsync(string id, DateTime scheduledDate);
        Task<Maintenance?> RateMaintenanceAsync(string id, double rating);
    }
}

