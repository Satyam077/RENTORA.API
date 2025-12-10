using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RENTARA.API.Models;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Models.Enums;
using RENTORA.API.Helpers;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.Services.IServices;
using RENTORA.API.WebSettings;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(
            ITenantsRepository tenantsRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<TenantsController> logger)
        {
            _tenantsRepository = tenantsRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseModel>> GetAllTenants()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                // Use repository method that joins Tenant and User data (no redundancy)
                var tenants = await _tenantsRepository.GetAllTenantsWithUserDataAsync();

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Tenants retrieved successfully";
                response.data = tenants;

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenants");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving tenants";
                return StatusCode(500, response);
            }
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> GetTenantById(string id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var tenant = await _tenantsRepository.GetTenantWithUserDataByIdAsync(id);

                if (tenant == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Tenant with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Tenant retrieved successfully";
                response.data = tenant;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenant with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving the tenant";
                return StatusCode(500, response);
            }
        }

        [HttpGet("owner/{ownerId}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseModel>> GetTenantsByOwnerId(string ownerId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var tenants = await _tenantsRepository.GetTenantsByOwnerIdWithUserDataAsync(ownerId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Tenants retrieved successfully";
                response.data = tenants;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenants for owner: {OwnerId}", ownerId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving tenants";
                return StatusCode(500, response);
            }
        }

        [HttpGet("property/{propertyId}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseModel>> GetTenantsByPropertyId(string propertyId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var tenants = await _tenantsRepository.GetTenantsByPropertyIdWithUserDataAsync(propertyId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Tenants retrieved successfully";
                response.data = tenants;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenants for property: {PropertyId}", propertyId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving tenants";
                return StatusCode(500, response);
            }
        }

        [HttpGet("unit/{unitId}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseModel>> GetTenantsByUnitId(string unitId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var tenants = await _tenantsRepository.GetTenantsByUnitIdWithUserDataAsync(unitId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Tenants retrieved successfully";
                response.data = tenants;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenants for unit: {UnitId}", unitId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving tenants";
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel>> CreateTenant([FromBody] TenantCreateDTO createDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid tenant data";
                    response.data = ModelState;
                    return BadRequest(response);
                }

                // Check if email already exists in Users collection
                var existingUser = await _userRepository.GetUserByEmailAsync(createDTO.Email);
                if (existingUser != null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = $"User with email '{createDTO.Email}' already exists";
                    return BadRequest(response);
                }

                // Check if mobile already exists in Users collection
                var existingMobile = await _userRepository.GetUserByMobileAsync(createDTO.Mobile);
                if (existingMobile != null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = $"User with mobile '{createDTO.Mobile}' already exists";
                    return BadRequest(response);
                }

                // Step 1: Generate secure random password
                string generatedPassword = PasswordGenerator.GenerateSecurePassword(12, true);

                // Step 2: Create password hash
                PasswordHelper.CreatePasswordHash(generatedPassword, out byte[] passwordHash, out byte[] passwordSalt);

                // Step 3: Create User Account in Registration collection
                var newUser = new Registration
                {
                    FullName = $"{createDTO.FirstName} {createDTO.LastName}",
                    Email = createDTO.Email,
                    Mobile = createDTO.Mobile,
                    Gender = createDTO.Gender,
                    DateOfBirth = createDTO.DateOfBirth,
                    PasswordHash = Convert.ToBase64String(passwordHash),
                    PasswordSalt = Convert.ToBase64String(passwordSalt),
                    Role = Role.Tenants,
                    OwnerId = createDTO.OwnerId,
                    IsEmailVerified = false,
                    IsMobileVerified = false,
                    IsOtpVerified = false,
                    CreatedBy = createDTO.CreatedBy,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.CreateUserAsync(newUser);

                // Step 4: Create Tenant record linked to User
                var tenant = new Tenant
                {
                    UserId = createdUser.Id, // Link to User account
                    OwnerId = createDTO.OwnerId,
                    PropertyId = createDTO.PropertyId,
                    UnitId = createDTO.UnitId,
                    PermanentAddress = createDTO.PermanentAddress,
                    CurrentAddress = createDTO.CurrentAddress,
                    RentAmount = createDTO.RentAmount,
                    SecurityDeposit = createDTO.SecurityDeposit,
                    RentDueDay = createDTO.RentDueDay,
                    AgreementStartDate = createDTO.AgreementStartDate,
                    AgreementEndDate = createDTO.AgreementEndDate,
                    Documents = createDTO.Documents,
                    IdProofType = createDTO.IdProofType,
                    IdProofNumber = createDTO.IdProofNumber,
                    MoveInDate = createDTO.MoveInDate,
                    Notes = createDTO.Notes,
                    CreatedBy = createDTO.CreatedBy
                };

                var createdTenant = await _tenantsRepository.CreateAsync(tenant);

                // Step 5: Update User with TenantId
                createdUser.TenantId = createdTenant.Id;
                await _userRepository.UpdateUserAsync(createdUser);

                // Step 6: Send Welcome Email with Credentials
                try
                {
                    var emailTokens = new Dictionary<string, string>
                    {
                        { "FullName", $"{createDTO.FirstName} {createDTO.LastName}" },
                        { "Email", createDTO.Email },
                        { "Password", generatedPassword },
                        { "PropertyName", createDTO.PropertyId },
                        { "ApplicationUrl", "http://localhost:4200/login" }, // Update with your actual login URL
                       { "SupportStaff", "RENTORA PMS" },
                       { "SupportContact", "+91 1234567899" },
                       { "SupportEmail", "uniquextech7@gmail.com"},
                       { "CurrentYear",DateTime.UtcNow.Year.ToString()}
                    };

                    await _emailService.SendTemplateEmailAsync(
                        createDTO.Email,
                        $"{createDTO.FirstName} {createDTO.LastName}",
                        EmailTemplateName.TenantsRegistration,
                        emailTokens
                    );

                    _logger.LogInformation($"Welcome email sent to tenant: {createDTO.Email}");
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, $"Failed to send welcome email to: {createDTO.Email}");
                    // Don't fail the entire operation if email fails
                }

                response.Success = true;
                response.Status = StatusCodes.Status201Created;
                response.Message = "Tenant created successfully. Login credentials sent to email.";
                response.data = new
                {
                    Tenant = createdTenant,
                    User = new
                    {
                        createdUser.Id,
                        createdUser.Email,
                        createdUser.Mobile,
                        createdUser.FullName
                    },
                    TemporaryPassword = generatedPassword // Only for development/testing, remove in production
                };

                return CreatedAtAction(
                    nameof(GetTenantById),
                    new { id = createdTenant.Id },
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tenant");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while creating the tenant";
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> UpdateTenant(string id, [FromBody] TenantUpdateDTO updateDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid tenant data";
                    response.data = ModelState;
                    return BadRequest(response);
                }

                if (id != updateDTO.Id)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "ID mismatch";
                    return BadRequest(response);
                }

                var existingTenant = await _tenantsRepository.GetByIdAsync(id);
                if (existingTenant == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Tenant with ID '{id}' not found";
                    return NotFound(response);
                }

                // Get the linked user account
                var existingUser = await _userRepository.GetUserByIdAsync(existingTenant.UserId);
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"User account for tenant not found";
                    return NotFound(response);
                }

                // Update User record (personal information)
                existingUser.FullName = $"{updateDTO.FirstName} {updateDTO.LastName}";
                existingUser.Email = updateDTO.Email;
                existingUser.Mobile = updateDTO.Mobile;
                existingUser.Gender = updateDTO.Gender;
                existingUser.DateOfBirth = updateDTO.DateOfBirth;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateUserAsync(existingUser);

                // Update Tenant record (tenant-specific information)
                var tenant = new Tenant
                {
                    Id = updateDTO.Id,
                    UserId = existingTenant.UserId, // Preserve the link
                    OwnerId = updateDTO.OwnerId,
                    PropertyId = updateDTO.PropertyId,
                    UnitId = updateDTO.UnitId,
                    PermanentAddress = updateDTO.PermanentAddress,
                    CurrentAddress = updateDTO.CurrentAddress,
                    RentAmount = updateDTO.RentAmount,
                    SecurityDeposit = updateDTO.SecurityDeposit,
                    RentDueDay = updateDTO.RentDueDay,
                    AgreementStartDate = updateDTO.AgreementStartDate,
                    AgreementEndDate = updateDTO.AgreementEndDate,
                    IsAgreementExpired = updateDTO.IsAgreementExpired,
                    Documents = updateDTO.Documents,
                    IdProofType = updateDTO.IdProofType,
                    IdProofNumber = updateDTO.IdProofNumber,
                    IsActiveTenant = updateDTO.IsActiveTenant,
                    IsRentPending = updateDTO.IsRentPending,
                    IsMovedOut = updateDTO.IsMovedOut,
                    MoveInDate = updateDTO.MoveInDate,
                    MoveOutDate = updateDTO.MoveOutDate,
                    Notes = updateDTO.Notes,
                    IsActive = updateDTO.IsActive,
                    UpdatedBy = updateDTO.UpdatedBy,
                    CreatedBy = existingTenant.CreatedBy,
                    CreatedAt = existingTenant.CreatedAt
                };

                var updatedTenant = await _tenantsRepository.UpdateAsync(tenant);

                if (updatedTenant == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Tenant with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Tenant updated successfully";
                response.data = new
                {
                    Tenant = updatedTenant,
                    User = new
                    {
                        existingUser.Id,
                        existingUser.Email,
                        existingUser.Mobile,
                        existingUser.FullName
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while updating the tenant";
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> DeleteTenant(string id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var result = await _tenantsRepository.DeleteAsync(id);

                if (!result)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Tenant with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Tenant deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tenant with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while deleting the tenant";
                return StatusCode(500, response);
            }
        }
    }
}
