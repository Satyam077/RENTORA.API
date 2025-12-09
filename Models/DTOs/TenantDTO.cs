using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class TenantCreateDTO
    {
        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string PropertyId { get; set; }

        [Required]
        public string UnitId { get; set; }

        // Personal Information (will be stored in Registration/Users collection)
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string FatherName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        // Tenant-specific fields (stored in Tenant collection)
        public string PermanentAddress { get; set; } = string.Empty;

        public string CurrentAddress { get; set; } = string.Empty;

        [Required]
        public decimal RentAmount { get; set; }

        public decimal SecurityDeposit { get; set; }

        public int RentDueDay { get; set; } = 5;

        [Required]
        public DateTime AgreementStartDate { get; set; }

        [Required]
        public DateTime AgreementEndDate { get; set; }

        public List<string> Documents { get; set; } = new List<string>();

        public string IdProofType { get; set; } = string.Empty;

        public string IdProofNumber { get; set; } = string.Empty;

        public DateTime? MoveInDate { get; set; }

        public string Notes { get; set; } = string.Empty;

        [Required]
        public string CreatedBy { get; set; }
    }

    public class TenantUpdateDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string PropertyId { get; set; }

        [Required]
        public string UnitId { get; set; }

        // Personal Information (will update Registration/Users collection)
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Phone]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string FatherName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        // Tenant-specific fields (stored in Tenant collection)
        public string PermanentAddress { get; set; } = string.Empty;

        public string CurrentAddress { get; set; } = string.Empty;

        [Required]
        public decimal RentAmount { get; set; }

        public decimal SecurityDeposit { get; set; }

        public int RentDueDay { get; set; } = 5;

        [Required]
        public DateTime AgreementStartDate { get; set; }

        [Required]
        public DateTime AgreementEndDate { get; set; }

        public bool IsAgreementExpired { get; set; }

        public List<string> Documents { get; set; } = new List<string>();

        public string IdProofType { get; set; } = string.Empty;

        public string IdProofNumber { get; set; } = string.Empty;

        public bool IsActiveTenant { get; set; } = true;

        public bool IsRentPending { get; set; }

        public bool IsMovedOut { get; set; } = false;

        public DateTime? MoveInDate { get; set; }

        public DateTime? MoveOutDate { get; set; }

        public string Notes { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Required]
        public string UpdatedBy { get; set; }
    }
}
