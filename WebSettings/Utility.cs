using Microsoft.IdentityModel.Tokens;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;
using RENTORA.API.Models.MongoDB;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RENTORA.API.WebSettings
{
    public class Utilities
    {       

        
    }

    public class PasswordHelper
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
    public static class RoleExtensions
    {
        public static string ToRoleString(this Role role)
        {
            return role switch
            {
                Role.SuperAdmin => "SuperAdmin",
                Role.Admin => "Admin",
                Role.Landlords => "Landlords",
                Role.Tenants => "Tenants",
                Role.Agents => "Agents",
                _ => "Unknown"
            };
        }
    }
    public static class Helper
    {
        public static string GetDisplayName(Enum value)
        {
            return value.GetType()
                        .GetField(value.ToString())
                        ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                        is DisplayAttribute[] da && da.Length > 0
                            ? da[0].Name
                            : value.ToString();
        }
    }
    public static class CommonMessage
    {
        public static class MessageSuccess
        {
            public const string Saved = "Record has been saved successfully.";
            public const string Success = "Your Registration has been successful.";
            public const string LoginSuccess = "Your Login has been successful.";
            public const string InActive = "Your Registration has been successful.";
            public const string OtpSent = "OTP sent successfully";
            public const string OtpVerified = "OTP verified successfully";
            public const string IsEmailSaved = "Email template updated successfully.";
            public const string IsPropertySaved = "Property saved successfully.";
        }

        public static class MessageError
        {
            public const string Failed = "Record has not been saved successfully.";
            public const string Invalid = "Invalid credentials.";
            public const string Required = "Email or Mobile is required.";
            public const string IsDuplicate = "User with this email or mobile already exists.";

            public const string PasswordNotMatch = "Passwords do not match.";
            public const string InActive = "Account is inactive. Please contact support.";
            public const string FailedOtp = "Failed to send OTP";
            public const string OtpExpired = "Invalid or expired OTP";
        }
    }
}
