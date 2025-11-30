using Microsoft.IdentityModel.Tokens;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;
using RENTORA.API.Models.MongoDB;
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
}
