using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Models.Enums;
using RENTORA.API.Models.MongoDB;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.Services.IServices;
using RENTORA.API.WebSettings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RENTORA.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings, IEmailService emailService)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
        }

        public async Task<LoginResponse> RegisterAsync(RegistrationDTO registrationDto)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(registrationDto.Email) && string.IsNullOrWhiteSpace(registrationDto.Mobile))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = CommonMessage.MessageError.Required
                    };
                }

                if (registrationDto.Password != registrationDto.ConfirmPassword)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = CommonMessage.MessageError.PasswordNotMatch
                    };
                }

                // Check if user already exists
                var existingUser = await _userRepository.GetUserByEmailOrMobileAsync(
                    registrationDto.Email ?? registrationDto.Mobile
                );

                if (existingUser != null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = CommonMessage.MessageError.IsDuplicate
                    };
                }
                PasswordHelper.CreatePasswordHash(registrationDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                
                var newUser = new Registration
                {
                    FullName = registrationDto.FullName,
                    Email = registrationDto.Email,
                    Mobile = registrationDto.Mobile,
                    Gender = registrationDto.Gender,
                    DateOfBirth = registrationDto.DateOfBirth,
                    PasswordHash = Convert.ToBase64String(passwordHash),
                    PasswordSalt = Convert.ToBase64String(passwordSalt),
                    Role = registrationDto.Role,
                    TenantId = registrationDto.TenantId,
                    OwnerId = registrationDto.OwnerId,
                    IsEmailVerified = false,
                    IsMobileVerified = false,
                    IsOtpVerified = false,
                    CreatedBy = "System",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.CreateUserAsync(newUser);

                // Generate JWT token
                var token = GenerateJwtToken(createdUser);

                return new LoginResponse
                {
                    Success = true,
                    Message = CommonMessage.MessageSuccess.Success,
                    Token = token,
                    User = new UserInfo
                    {
                        Id = createdUser.Id,
                        FullName = createdUser.FullName,
                        Email = createdUser.Email,
                        Mobile = createdUser.Mobile,
                        Role = createdUser.Role,
                        ProfileImageUrl = createdUser.ProfileImageUrl,
                        IsEmailVerified = createdUser.IsEmailVerified,
                        IsMobileVerified = createdUser.IsMobileVerified
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                // Find user by email or mobile
                var user = await _userRepository.GetUserByEmailOrMobileAsync(loginDto.EmailOrMobile);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = CommonMessage.MessageError.Invalid
                    };
                }

                // Verify password
                if (!PasswordHelper.VerifyPasswordHash(loginDto.Password,
                    Convert.FromBase64String(user.PasswordHash),Convert.FromBase64String(user.PasswordSalt)))
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = CommonMessage.MessageError.Invalid
                    };
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = CommonMessage.MessageError.InActive
                    };
                }

                // Generate JWT token
                var token = GenerateJwtToken(user);

                return new LoginResponse
                {
                    Success = true,
                    Message = CommonMessage.MessageSuccess.LoginSuccess,
                    Token = token,
                    User = new UserInfo
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Mobile = user.Mobile,
                        Role = user.Role,
                        ProfileImageUrl = user.ProfileImageUrl,
                        IsEmailVerified = user.IsEmailVerified,
                        IsMobileVerified = user.IsMobileVerified
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> SendOtpAsync(string emailOrMobile)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailOrMobileAsync(emailOrMobile);
                if (user == null)
                    return false;

                // Generate 6-digit OTP
                var otp = new Random().Next(100000, 999999).ToString();

                user.LastOtpCode = otp;
                user.LastOtpGeneratedAt = DateTime.UtcNow;
                user.IsOtpVerified = false;

                await _userRepository.UpdateUserAsync(user);

                // TODO: Send OTP via email or SMS
                // For now, just log it (in production, integrate with email/SMS service)
                Console.WriteLine($"OTP for {emailOrMobile}: {otp}");

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailOrMobileAsync(request.EmailOrMobile);
                if (user == null)
                    return false;

                // Check if OTP is valid
                if (user.LastOtpCode != request.OtpCode)
                    return false;

                // Check if OTP is expired (valid for 10 minutes)
                if (user.LastOtpGeneratedAt.HasValue &&
                    DateTime.UtcNow.Subtract(user.LastOtpGeneratedAt.Value).TotalMinutes > 10)
                    return false;

                // Mark OTP as verified
                user.IsOtpVerified = true;

                // Mark email or mobile as verified
                if (!string.IsNullOrEmpty(user.Email) && user.Email == request.EmailOrMobile)
                    user.IsEmailVerified = true;

                if (!string.IsNullOrEmpty(user.Mobile) && user.Mobile == request.EmailOrMobile)
                    user.IsMobileVerified = true;

                await _userRepository.UpdateUserAsync(user);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public string GenerateJwtToken(Registration user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user.Mobile ?? string.Empty),
                new Claim("ProfileImageUrl", user.ProfileImageUrl ?? string.Empty),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(Role), user.Role)!)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
