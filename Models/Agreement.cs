using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;
using System;

namespace RENTARA.API.Models
{
    public class Agreement : BaseEntity
    {
        public string PropertyId { get; set; }

        public string UnitId { get; set; }

        public string TenantId { get; set; }

        public string OwnerId { get; set; }

        public string AgreementNumber { get; set; }
        public AgreementType AgreementType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Financial Snapshot (locked at agreement time)
        public decimal RentAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public int RentDueDay { get; set; } = 5;

        // Agreement File
        public string AgreementFileUrl { get; set; }  // PDF/Image stored in cloud

        // Status
        public AgreementStatus Status { get; set; } = AgreementStatus.Active;

        // Termination
        public DateTime? TerminatedOn { get; set; }
        public string TerminationReason { get; set; }

        // Renewal
        public bool IsRenewed { get; set; } = false;
        public string RenewedFromAgreementId { get; set; }

        // Notes
        public string Notes { get; set; }
    }
}
