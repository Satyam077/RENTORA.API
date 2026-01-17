using RENTORA.API.Models.Enums;

namespace RENTORA.API.Models.DTOs
{
    public class GoogleAuthDTO
    {
        public string IdToken { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.Tenants;
        public bool IsRegistration { get; set; } = false;
    }

    public class GoogleAuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsNewUser { get; set; }
        public GoogleUserInfo? GoogleUser { get; set; }
        public string Token { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
    }

    public class GoogleUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public string Mobile { get; set; } =  string.Empty;
    }
}
