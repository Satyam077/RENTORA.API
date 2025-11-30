using System.ComponentModel.DataAnnotations;

namespace RENTORA.API.Models.DTOs
{
    public class RegistrationDTO
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Role: superadmin, owner, manager, tenant
        public string Role { get; set; }

        // Optional values for linking
        public string? TenantId { get; set; } = string.Empty;
        public string? OwnerId { get; set; } = string.Empty;
    }
}
