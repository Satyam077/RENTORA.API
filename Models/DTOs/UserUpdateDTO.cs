using RENTORA.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "User ID is required")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; }

        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number")]
        public string? Mobile { get; set; }

        public string? ProfileImageUrl { get; set; }

        public AddressDTO? Address { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Role Role { get; set; }

        public string? TenantId { get; set; }

        public string? OwnerId { get; set; }

        public bool? IsActive { get; set; }
    }
}

