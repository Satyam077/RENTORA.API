using RENTORA.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class AgreementCreateDTO
    {
        [Required]
        public string PropertyId { get; set; }

        [Required]
        public string UnitId { get; set; }

        [Required]
        public string TenantId { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string AgreementNumber { get; set; }

        [Required]
        public AgreementType AgreementType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal RentAmount { get; set; }

        [Required]
        public decimal SecurityDeposit { get; set; }

        public int RentDueDay { get; set; } = 5;

        public string AgreementFileUrl { get; set; } = string.Empty;

        public AgreementStatus Status { get; set; } = AgreementStatus.Active;

        public string Notes { get; set; } = string.Empty;

        [Required]
        public string CreatedBy { get; set; }
    }

    public class AgreementUpdateDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string PropertyId { get; set; }

        [Required]
        public string UnitId { get; set; }

        [Required]
        public string TenantId { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string AgreementNumber { get; set; }

        [Required]
        public AgreementType AgreementType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal RentAmount { get; set; }

        [Required]
        public decimal SecurityDeposit { get; set; }

        public int RentDueDay { get; set; } = 5;

        public string AgreementFileUrl { get; set; } = string.Empty;

        public AgreementStatus Status { get; set; } = AgreementStatus.Active;

        public DateTime? TerminatedOn { get; set; }

        public string TerminationReason { get; set; } = string.Empty;

        public bool IsRenewed { get; set; } = false;

        public string RenewedFromAgreementId { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Required]
        public string UpdatedBy { get; set; }
    }

    public class AgreementResponseDTO
    {
        public string Id { get; set; }
        public string PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string UnitId { get; set; }
        public string UnitName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string OwnerId { get; set; }
        public string AgreementNumber { get; set; }
        public AgreementType AgreementType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal RentAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public int RentDueDay { get; set; }
        public string AgreementFileUrl { get; set; }
        public AgreementStatus Status { get; set; }
        public DateTime? TerminatedOn { get; set; }
        public string TerminationReason { get; set; }
        public bool IsRenewed { get; set; }
        public string RenewedFromAgreementId { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
