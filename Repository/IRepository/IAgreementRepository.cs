using RENTARA.API.Models;
using RENTORA.API.Models.DTOs;

namespace RENTORA.API.Repository.IRepository
{
    public interface IAgreementRepository
    {
        // Basic CRUD operations
        Task<IEnumerable<Agreement>> GetAllAsync();
        Task<Agreement?> GetByIdAsync(string id);
        Task<IEnumerable<Agreement>> GetByOwnerIdAsync(string ownerId);
        Task<IEnumerable<Agreement>> GetByPropertyIdAsync(string propertyId);
        Task<IEnumerable<Agreement>> GetByUnitIdAsync(string unitId);
        Task<IEnumerable<Agreement>> GetByTenantIdAsync(string tenantId);
        Task<Agreement> CreateAsync(Agreement agreement);
        Task<Agreement?> UpdateAsync(Agreement agreement);
        Task<bool> DeleteAsync(string id);

        // Methods with joined data (Property, Unit, Tenant names)
        Task<IEnumerable<AgreementResponseDTO>> GetAllAgreementsWithDetailsAsync();
        Task<AgreementResponseDTO?> GetAgreementWithDetailsByIdAsync(string id);
        Task<IEnumerable<AgreementResponseDTO>> GetAgreementsByOwnerIdWithDetailsAsync(string ownerId);
        Task<IEnumerable<AgreementResponseDTO>> GetAgreementsByPropertyIdWithDetailsAsync(string propertyId);
        Task<IEnumerable<AgreementResponseDTO>> GetAgreementsByUnitIdWithDetailsAsync(string unitId);
        Task<IEnumerable<AgreementResponseDTO>> GetAgreementsByTenantIdWithDetailsAsync(string tenantId);
    }
}
