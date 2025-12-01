using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Models.Enums;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.WebSettings;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if user already exists
                var existingUser = await _userRepository.GetUserByEmailOrMobileAsync(
                    userDto.Email ?? userDto.Mobile
                );

                if (existingUser != null)
                {
                    return BadRequest(new { success = false, message = "User with this email or mobile already exists" });
                }

                // Create password hash and salt
                PasswordHelper.CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

                // Create address if provided
                Address? address = null;
                if (userDto.Address != null && 
                    (!string.IsNullOrWhiteSpace(userDto.Address.AddressLine1) || 
                     !string.IsNullOrWhiteSpace(userDto.Address.City)))
                {
                    address = new Address
                    {
                        AddressLine1 = userDto.Address.AddressLine1,
                        AddressLine2 = userDto.Address.AddressLine2,
                        City = userDto.Address.City,
                        State = userDto.Address.State,
                        Country = userDto.Address.Country,
                        ZipCode = userDto.Address.ZipCode
                    };
                }

                // Create new user
                var newUser = new Registration
                {
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Mobile = userDto.Mobile,
                    Gender = userDto.Gender,
                    DateOfBirth = userDto.DateOfBirth,
                    PasswordHash = Convert.ToBase64String(passwordHash),
                    PasswordSalt = Convert.ToBase64String(passwordSalt),
                    ProfileImageUrl = userDto.ProfileImageUrl,
                    Address = address,
                    Role = userDto.Role,
                    TenantId = userDto.TenantId,
                    OwnerId = userDto.OwnerId,
                    IsEmailVerified = false,
                    IsMobileVerified = false,
                    IsOtpVerified = false,
                    CreatedBy = User.Identity?.Name ?? Enum.GetName(Role.SuperAdmin),
                    IsActive = true,
                    IsDeleted = false
                };

                var createdUser = await _userRepository.CreateUserAsync(newUser);

                // Return user without sensitive data
                return Ok(new
                {
                    success = true,
                    message = "User created successfully",
                    user = new
                    {
                        id = createdUser.Id,
                        fullName = createdUser.FullName,
                        email = createdUser.Email,
                        mobile = createdUser.Mobile,
                        role = createdUser.Role,
                        isActive = createdUser.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Failed to create user: {ex.Message}" });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get existing user
                var existingUser = await _userRepository.GetUserByIdAsync(userDto.Id);

                if (existingUser == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Check if email or mobile is being changed and if it's already taken
                if (existingUser.Email != userDto.Email || existingUser.Mobile != userDto.Mobile)
                {
                    var userWithSameEmailOrMobile = await _userRepository.GetUserByEmailOrMobileAsync(
                        userDto.Email ?? userDto.Mobile
                    );

                    if (userWithSameEmailOrMobile != null && userWithSameEmailOrMobile.Id != userDto.Id)
                    {
                        return BadRequest(new { success = false, message = "Email or mobile is already in use by another user" });
                    }
                }

                // Update address if provided
                Address? address = null;
                if (userDto.Address != null && 
                    (!string.IsNullOrWhiteSpace(userDto.Address.AddressLine1) || 
                     !string.IsNullOrWhiteSpace(userDto.Address.City)))
                {
                    address = new Address
                    {
                        Id = existingUser.Address?.Id,
                        AddressLine1 = userDto.Address.AddressLine1,
                        AddressLine2 = userDto.Address.AddressLine2,
                        City = userDto.Address.City,
                        State = userDto.Address.State,
                        Country = userDto.Address.Country,
                        ZipCode = userDto.Address.ZipCode
                    };
                }

                // Update user properties
                existingUser.FullName = userDto.FullName;
                existingUser.Email = userDto.Email;
                existingUser.Mobile = userDto.Mobile;
                existingUser.Gender = userDto.Gender;
                existingUser.DateOfBirth = userDto.DateOfBirth;
                existingUser.ProfileImageUrl = userDto.ProfileImageUrl;
                existingUser.Address = address;
                existingUser.Role = userDto.Role;
                existingUser.TenantId = userDto.TenantId;
                existingUser.OwnerId = userDto.OwnerId;
                existingUser.IsActive = userDto.IsActive ?? existingUser.IsActive;

                var result = await _userRepository.UpdateUserAsync(existingUser);

                if (!result)
                {
                    return StatusCode(500, new { success = false, message = "Failed to update user" });
                }

                return Ok(new
                {
                    success = true,
                    message = "User updated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Failed to update user: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Return user without sensitive data
                return Ok(new
                {
                    success = true,
                    user = new
                    {
                        id = user.Id,
                        fullName = user.FullName,
                        gender = user.Gender,
                        dateOfBirth = user.DateOfBirth,
                        email = user.Email,
                        isEmailVerified = user.IsEmailVerified,
                        mobile = user.Mobile,
                        isMobileVerified = user.IsMobileVerified,
                        profileImageUrl = user.ProfileImageUrl,
                        address = user.Address,
                        role = user.Role,
                        tenantId = user.TenantId,
                        ownerId = user.OwnerId,
                        isActive = user.IsActive,
                        createdAt = user.CreatedAt,
                        updatedAt = user.UpdatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Failed to get user: {ex.Message}" });
            }
        }

        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();

                var userList = users.Select(u => new
                {
                    id = u.Id,
                    fullName = u.FullName,
                    email = u.Email,
                    mobile = u.Mobile,
                    role = u.Role,
                    gender = u.Gender,
                    dateOfBirth = u.DateOfBirth,
                    isEmailVerified = u.IsEmailVerified,
                    isMobileVerified = u.IsMobileVerified,
                    profileImageUrl = u.ProfileImageUrl,
                    address = u.Address,
                    tenantId = u.TenantId,
                    ownerId = u.OwnerId,
                    isActive = u.IsActive,
                    createdAt = u.CreatedAt
                }).ToList();

                return Ok(new
                {
                    success = true,
                    users = userList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Failed to get users: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _userRepository.DeleteUserAsync(id);

                if (!result)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                return Ok(new
                {
                    success = true,
                    message = "User deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Failed to delete user: {ex.Message}" });
            }
        }

        [HttpPost("upload-profile-picture/{userId}")]
        public async Task<IActionResult> UploadProfilePicture(string userId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { success = false, message = "No file uploaded" });
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, message = "Invalid file type. Only images are allowed." });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { success = false, message = "File size exceeds 5MB limit" });
                }

                // Get user
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                // Create uploads directory if it doesn't exist
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Delete old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var oldFilePath = user.ProfileImageUrl.Replace("/uploads/", "").Replace("/", "\\");
                    var fullOldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldFilePath);
                    if (System.IO.File.Exists(fullOldPath))
                    {
                        try
                        {
                            System.IO.File.Delete(fullOldPath);
                        }
                        catch { /* Ignore deletion errors */ }
                    }
                }

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update user profile image URL
                var imageUrl = $"/uploads/profiles/{fileName}";
                user.ProfileImageUrl = imageUrl;
                await _userRepository.UpdateUserAsync(user);

                return Ok(new
                {
                    success = true,
                    message = "Profile picture uploaded successfully",
                    imageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Failed to upload profile picture: {ex.Message}" });
            }
        }
    }
}

