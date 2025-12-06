using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RENTORA.API.Models.Enums;

namespace RENTORA.API.Models
{
    [BsonCollection("Users")]
    public class Registration : BaseEntity
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Mobile { get; set; }
        public bool IsMobileVerified { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string LastOtpCode { get; set; }
        public DateTime? LastOtpGeneratedAt { get; set; }
        public bool IsOtpVerified { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ApplicationUrl { get; set; } =  string.Empty;
        public Address Address { get; set; }

        // Multi-Role Support: superadmin, owner, manager, tenant
        public Role Role { get; set; }
        public string? TenantId { get; set; } = string.Empty;
        public string? OwnerId { get; set; } = string.Empty;
    }
    public class Address
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}
