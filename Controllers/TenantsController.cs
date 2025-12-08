using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RENTARA.API.Models;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.WebSettings;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(
            ITenantsRepository tenantsRepository,
            ILogger<TenantsController> logger)
        {
            _tenantsRepository = tenantsRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseModel>> GetAllTenants()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var tenants = await _tenantsRepository.GetAllAsync();
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
                var tenant = await _tenantsRepository.GetByIdAsync(id);

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
                var tenants = await _tenantsRepository.GetByOwnerIdAsync(ownerId);
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
                var tenants = await _tenantsRepository.GetByPropertyIdAsync(propertyId);
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
                var tenants = await _tenantsRepository.GetByUnitIdAsync(unitId);
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

                // Check if email already exists
                var emailExists = await _tenantsRepository.ExistsByEmailAsync(createDTO.Email);
                if (emailExists)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = $"Tenant with email '{createDTO.Email}' already exists";
                    return BadRequest(response);
                }

                // Check if mobile already exists
                var mobileExists = await _tenantsRepository.ExistsByMobileAsync(createDTO.Mobile);
                if (mobileExists)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = $"Tenant with mobile '{createDTO.Mobile}' already exists";
                    return BadRequest(response);
                }

                // Map DTO to Tenant
                var tenant = new Tenant
                {
                    OwnerId = createDTO.OwnerId,
                    PropertyId = createDTO.PropertyId,
                    UnitId = createDTO.UnitId,
                    FirstName = createDTO.FirstName,
                    LastName = createDTO.LastName,
                    Mobile = createDTO.Mobile,
                    Email = createDTO.Email,
                    Password = createDTO.Password,
                    Gender = createDTO.Gender,
                    FatherName = createDTO.FatherName,
                    DateOfBirth = createDTO.DateOfBirth,
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

                response.Success = true;
                response.Status = StatusCodes.Status201Created;
                response.Message = "Tenant created successfully";
                response.data = createdTenant;

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

                // Map DTO to Tenant
                var tenant = new Tenant
                {
                    Id = updateDTO.Id,
                    OwnerId = updateDTO.OwnerId,
                    PropertyId = updateDTO.PropertyId,
                    UnitId = updateDTO.UnitId,
                    FirstName = updateDTO.FirstName,
                    LastName = updateDTO.LastName,
                    Mobile = updateDTO.Mobile,
                    Email = updateDTO.Email,
                    Password = updateDTO.Password,
                    Gender = updateDTO.Gender,
                    FatherName = updateDTO.FatherName,
                    DateOfBirth = updateDTO.DateOfBirth,
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
                response.data = updatedTenant;
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
