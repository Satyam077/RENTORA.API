namespace RENTORA.API.Models.DTOs
{
    /// <summary>
    /// DTO for Tenant Dashboard containing all necessary information
    /// </summary>
    public class TenantDashboardDTO
    {
        // Tenant Information
        public string TenantId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string ProfileImageUrl { get; set; }

        // Property Information
        public string PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyAddress { get; set; }

        // Unit Information
        public string UnitId { get; set; }
        public string UnitName { get; set; }

        // Lease Information
        public DateTime AgreementStartDate { get; set; }
        public DateTime AgreementEndDate { get; set; }
        public bool IsAgreementExpired { get; set; }
        public int DaysUntilLeaseEnd { get; set; }

        // Rent Information
        public decimal RentAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public int RentDueDay { get; set; }
        public DateTime NextRentDueDate { get; set; }
        public bool IsRentPending { get; set; }
        public int PaymentProgress { get; set; } // 0-100 percentage

        // Status
        public bool IsActiveTenant { get; set; }
        public DateTime? MoveInDate { get; set; }
    }
}
