using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RENTORA.API.Models;

namespace RENTARA.API.Models
{
    public class Tenant : BaseEntity
    {
        public string PropertyId { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Otp { get; set; }

        public string Gender { get; set; }
        public string FatherName { get; set; }    // optional
        public DateTime? DateOfBirth { get; set; }

        // Address Details
        public string PermanentAddress { get; set; }
        public string CurrentAddress { get; set; }

        // Rent & Agreement Info
        public decimal RentAmount { get; set; }
        public decimal SecurityDeposit { get; set; }
        public int RentDueDay { get; set; } = 5;

        public DateTime AgreementStartDate { get; set; }
        public DateTime AgreementEndDate { get; set; }
        public bool IsAgreementExpired { get; set; }

        // KYC / ID Proof
        public List<string> Documents { get; set; } = new List<string>();  // URLs to images/PDFs

        public string IdProofType { get; set; }           // Aadhaar, PAN, Passport, etc.
        public string IdProofNumber { get; set; }

        // Status Tracking
        public bool IsActiveTenant { get; set; } = true;
        public bool IsRentPending { get; set; }
        public bool IsMovedOut { get; set; } = false;

        public DateTime? MoveInDate { get; set; }
        public DateTime? MoveOutDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
