using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;

namespace RENTORA.API.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> RegisterAsync(RegistrationDTO registrationDto);
        Task<LoginResponse> LoginAsync(LoginDTO loginDto);
        Task<bool> SendOtpAsync(string emailOrMobile);
        Task<bool> VerifyOtpAsync(VerifyOtpRequest request);
        string GenerateJwtToken(Registration user);
    }
}
