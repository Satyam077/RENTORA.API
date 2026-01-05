using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RENTARA.API.Models;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Models.Enums;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.Services.IServices;
using RENTORA.API.WebSettings;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AgreementController : ControllerBase
    {
        private readonly IAgreementRepository _agreementRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<AgreementController> _logger;
        private readonly IEmailService _emailService;

        public AgreementController(
            IAgreementRepository agreementRepository,
            IFileUploadService fileUploadService,
            ILogger<AgreementController> logger,
            IEmailService emailService
            
            )
        {
            _agreementRepository = agreementRepository;
            _fileUploadService = fileUploadService;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel>> GetAllAgreements()
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var agreements = await _agreementRepository.GetAllAgreementsWithDetailsAsync();

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Agreements retrieved successfully";
                response.data = agreements;

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agreements");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving agreements";
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel>> GetAgreementById(string id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var agreement = await _agreementRepository.GetAgreementWithDetailsByIdAsync(id);

                if (agreement == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Agreement with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Agreement retrieved successfully";
                response.data = agreement;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agreement with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving the agreement";
                return StatusCode(500, response);
            }
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<ResponseModel>> GetAgreementsByOwnerId(string ownerId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var agreements = await _agreementRepository.GetAgreementsByOwnerIdWithDetailsAsync(ownerId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Agreements retrieved successfully";
                response.data = agreements;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agreements for owner: {OwnerId}", ownerId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving agreements";
                return StatusCode(500, response);
            }
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<ResponseModel>> GetAgreementsByPropertyId(string propertyId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var agreements = await _agreementRepository.GetAgreementsByPropertyIdWithDetailsAsync(propertyId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Agreements retrieved successfully";
                response.data = agreements;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agreements for property: {PropertyId}", propertyId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving agreements";
                return StatusCode(500, response);
            }
        }

        [HttpGet("unit/{unitId}")]
        public async Task<ActionResult<ResponseModel>> GetAgreementsByUnitId(string unitId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var agreements = await _agreementRepository.GetAgreementsByUnitIdWithDetailsAsync(unitId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Agreements retrieved successfully";
                response.data = agreements;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agreements for unit: {UnitId}", unitId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving agreements";
                return StatusCode(500, response);
            }
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<ActionResult<ResponseModel>> GetAgreementsByTenantId(string tenantId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var agreements = await _agreementRepository.GetAgreementsByTenantIdWithDetailsAsync(tenantId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Agreements retrieved successfully";
                response.data = agreements;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agreements for tenant: {TenantId}", tenantId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving agreements";
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel>> CreateAgreement([FromBody] AgreementCreateDTO createDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid agreement data";
                    response.data = ModelState;
                    return BadRequest(response);
                }

                var agreement = new Agreement
                {
                    PropertyId = createDTO.PropertyId,
                    UnitId = createDTO.UnitId,
                    TenantId = createDTO.TenantId,
                    OwnerId = createDTO.OwnerId,
                    AgreementNumber = createDTO.AgreementNumber,
                    AgreementType = createDTO.AgreementType,
                    StartDate = createDTO.StartDate,
                    EndDate = createDTO.EndDate,
                    RentAmount = createDTO.RentAmount,
                    SecurityDeposit = createDTO.SecurityDeposit,
                    RentDueDay = createDTO.RentDueDay,
                    AgreementFileUrl = createDTO.AgreementFileUrl,
                    Status = createDTO.Status,
                    Notes = createDTO.Notes,
                    CreatedBy = createDTO.CreatedBy
                };

                var createdAgreement = await _agreementRepository.CreateAsync(agreement);

                try
                {
                    var emailTokens = new Dictionary<string, string>
                    {
                        { "FullName", $"{createDTO.OwnerId}" },
                        { "Email", "tenants@yopmail.com" },
                        { "PropertyName", createDTO.PropertyId },
                        { "PropertyName", createDTO.AgreementNumber },
                        { "PropertyName", createDTO.AgreementType.ToString() },
                       { "SupportStaff", "RENTORA PMS" },
                       { "SupportContact", "+91 1234567899" },
                       { "SupportEmail", "uniquextech7@gmail.com"},
                       { "CurrentYear",DateTime.UtcNow.Year.ToString()}
                    };

                    await _emailService.SendTemplateEmailAsync(
                        "tenants@yopmail.com",
                        $"{createDTO.OwnerId} {createDTO.TenantId}",
                        EmailTemplateName.AgentRegistration,
                        emailTokens
                    );

                    _logger.LogInformation($"Welcome email sent to tenant: {"tenants@yopmail.com"}");
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, $"Failed to send welcome email to: {"tenants@yopmail.com"}");
                }

                response.Success = true;
                response.Status = StatusCodes.Status201Created;
                response.Message = "Agreement created successfully";
                response.data = createdAgreement;

                return CreatedAtAction(
                    nameof(GetAgreementById),
                    new { id = createdAgreement.Id },
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating agreement");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while creating the agreement";
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel>> UpdateAgreement(string id, [FromBody] AgreementUpdateDTO updateDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid agreement data";
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

                var existingAgreement = await _agreementRepository.GetByIdAsync(id);
                if (existingAgreement == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Agreement with ID '{id}' not found";
                    return NotFound(response);
                }

                var agreement = new Agreement
                {
                    Id = updateDTO.Id,
                    PropertyId = updateDTO.PropertyId,
                    UnitId = updateDTO.UnitId,
                    TenantId = updateDTO.TenantId,
                    OwnerId = updateDTO.OwnerId,
                    AgreementNumber = updateDTO.AgreementNumber,
                    AgreementType = updateDTO.AgreementType,
                    StartDate = updateDTO.StartDate,
                    EndDate = updateDTO.EndDate,
                    RentAmount = updateDTO.RentAmount,
                    SecurityDeposit = updateDTO.SecurityDeposit,
                    RentDueDay = updateDTO.RentDueDay,
                    AgreementFileUrl = updateDTO.AgreementFileUrl,
                    Status = updateDTO.Status,
                    TerminatedOn = updateDTO.TerminatedOn,
                    TerminationReason = updateDTO.TerminationReason,
                    IsRenewed = updateDTO.IsRenewed,
                    RenewedFromAgreementId = updateDTO.RenewedFromAgreementId,
                    Notes = updateDTO.Notes,
                    IsActive = updateDTO.IsActive,
                    UpdatedBy = updateDTO.UpdatedBy,
                    CreatedBy = existingAgreement.CreatedBy,
                    CreatedAt = existingAgreement.CreatedAt
                };

                var updatedAgreement = await _agreementRepository.UpdateAsync(agreement);

                if (updatedAgreement == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Agreement with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Agreement updated successfully";
                response.data = updatedAgreement;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agreement with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while updating the agreement";
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel>> DeleteAgreement(string id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var result = await _agreementRepository.DeleteAsync(id);

                if (!result)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Agreement with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Agreement deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting agreement with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while deleting the agreement";
                return StatusCode(500, response);
            }
        }

        [HttpPost("upload-document")]
        public async Task<ActionResult<ResponseModel>> UploadAgreementDocument(IFormFile file)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "No file uploaded";
                    return BadRequest(response);
                }

                // Allowed file types for agreements
                string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
                
                // Validate file type
                if (!_fileUploadService.IsValidFileType(file, allowedExtensions))
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = $"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}";
                    return BadRequest(response);
                }

                // Validate file size (10MB max)
                if (!_fileUploadService.IsValidFileSize(file, 10))
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "File size exceeds 10MB limit";
                    return BadRequest(response);
                }

                // Upload file
                var fileUrl = await _fileUploadService.UploadFileAsync(file, "agreements");

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "File uploaded successfully";
                response.data = new { fileUrl = fileUrl };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading agreement document");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = $"An error occurred while uploading the file: {ex.Message}";
                return StatusCode(500, response);
            }
        }
    }
}
