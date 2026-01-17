using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;

namespace RENTORA.API.Services.IServices
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegistrationDTO registrationDto);
        Task<LoginResponse> LoginAsync(LoginDTO loginDto);
        Task<GoogleAuthResponse> GoogleAuthAsync(GoogleAuthDTO googleAuthDto);
        Task<bool> SendOtpAsync(string emailOrMobile);
        Task<bool> VerifyOtpAsync(VerifyOtpRequest request);
        string GenerateJwtToken(Registration user);
    }
}
