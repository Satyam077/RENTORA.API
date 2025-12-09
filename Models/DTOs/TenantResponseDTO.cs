namespace RENTORA.API.Models.DTOs
{
    /// <summary>
    /// Response DTO that combines Tenant and User (Registration) data
    /// Eliminates redundancy by joining data from both collections
    /// </summary>
    public class TenantResponseDTO
    {
        // Tenant ID
        public string Id { get; set; }

        // User Information (from Registration/Users collection)
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsMobileVerified { get; set; }
        public string ProfileImageUrl { get; set; }

        // Tenant-specific Information (from Tenants collection)
        public string OwnerId { get; set; }
        public string PropertyId { get; set; }
        public string UnitId { get; set; }
        public string PermanentAddress { get; set; }
        public string CurrentAddress { get; set; }

        // Rent & Agreement Info
        public decimal RentAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public int RentDueDay { get; set; }
        public DateTime AgreementStartDate { get; set; }
        public DateTime AgreementEndDate { get; set; }
        public bool IsAgreementExpired { get; set; }

        // KYC / ID Proof
        public List<string> Documents { get; set; } = new List<string>();
        public string IdProofType { get; set; }
        public string IdProofNumber { get; set; }

        // Status Tracking
        public bool IsActiveTenant { get; set; }
        public bool IsRentPending { get; set; }
        public bool IsMovedOut { get; set; }
        public DateTime? MoveInDate { get; set; }
        public DateTime? MoveOutDate { get; set; }
        public string Notes { get; set; }

        // Base Entity Fields
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
