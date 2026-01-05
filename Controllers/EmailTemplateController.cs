using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Repository.IRepository;
using RENTORA.API.WebSettings;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly ILogger<EmailTemplateController> _logger;

        public EmailTemplateController(
            IEmailTemplateRepository emailTemplateRepository,
            ILogger<EmailTemplateController> logger)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmailTemplateResponseDTO>>> GetAllTemplates()
        {
            try
            {
                var templates = await _emailTemplateRepository.GetAllAsync();
                var response = templates.Select(MapToResponseDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving email templates");
                return StatusCode(500, "An error occurred while retrieving email templates");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmailTemplateResponseDTO>> GetTemplateById(string id)
        {
            try
            {
                var template = await _emailTemplateRepository.GetByIdAsync(id);
                
                if (template == null)
                {
                    return NotFound($"Email template with ID '{id}' not found");
                }

                return Ok(MapToResponseDTO(template));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving email template with ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the email template");
            }
        }

        [HttpGet("applicable/{applicableFor}")]
        public async Task<ActionResult<IEnumerable<EmailTemplateResponseDTO>>> GetTemplatesByApplicableFor(string applicableFor)
        {
            try
            {
                var templates = await _emailTemplateRepository.GetByApplicableForAsync(applicableFor);
                var response = templates.Select(MapToResponseDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving email templates for ApplicableFor: {ApplicableFor}", applicableFor);
                return StatusCode(500, "An error occurred while retrieving email templates");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EmailTemplateResponseDTO>> CreateTemplate([FromBody] EmailTemplateCreateDTO createDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if template name already exists
                var exists = await _emailTemplateRepository.ExistsAsync(createDTO.TemplateName);
                if (exists)
                {
                    return BadRequest($"Email template with name '{createDTO.TemplateName}' already exists");
                }

                var template = new EmailTemplate
                {
                    Tokens = createDTO.Tokens,
                    TemplateName = createDTO.TemplateName,
                    EmailSubject = createDTO.EmailSubject,
                    EmailBody = createDTO.EmailBody,
                    ApplicableFor = createDTO.ApplicableFor
                };

                var createdTemplate = await _emailTemplateRepository.CreateAsync(template);
                var response = MapToResponseDTO(createdTemplate);

                return CreatedAtAction(
                    nameof(GetTemplateById),
                    new { id = createdTemplate.Id },
                    new
                    {
                        success = true,
                        message = CommonMessage.MessageSuccess.IsEmailSaved,
                        data = response
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating email template");
                return StatusCode(500, "An error occurred while creating the email template");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmailTemplateResponseDTO>> UpdateTemplate(string id, [FromBody] EmailTemplateUpdateDTO updateDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != updateDTO.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingTemplate = await _emailTemplateRepository.GetByIdAsync(id);
                if (existingTemplate == null)
                {
                    return NotFound($"Email template with ID '{id}' not found");
                }

                var template = new EmailTemplate
                {
                    Id = updateDTO.Id,
                    Tokens = updateDTO.Tokens,
                    TemplateName = updateDTO.TemplateName,
                    EmailSubject = updateDTO.EmailSubject,
                    EmailBody = updateDTO.EmailBody,
                    ApplicableFor = updateDTO.ApplicableFor,
                    IsActive = updateDTO.IsActive
                };

                var updatedTemplate = await _emailTemplateRepository.UpdateAsync(template);
                
                if (updatedTemplate == null)
                {
                    return NotFound($"Email template with ID '{id}' not found");
                }

               // return Ok(MapToResponseDTO(updatedTemplate));
                return Ok(new
                {
                    success = true,
                    message = CommonMessage.MessageSuccess.IsEmailSaved,
                    data = MapToResponseDTO(updatedTemplate)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email template with ID: {Id}", id);
                return StatusCode(500, "An error occurred while updating the email template");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(string id)
        {
            try
            {
                var result = await _emailTemplateRepository.DeleteAsync(id);
                
                if (!result)
                {
                    return NotFound($"Email template with ID '{id}' not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting email template with ID: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the email template");
            }
        }

        [HttpGet("exists/{templateName}")]
        public async Task<ActionResult<bool>> CheckTemplateExists(string templateName)
        {
            try
            {
                var exists = await _emailTemplateRepository.ExistsAsync(templateName);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if template exists: {TemplateName}", templateName);
                return StatusCode(500, "An error occurred while checking template existence");
            }
        }

        private EmailTemplateResponseDTO MapToResponseDTO(EmailTemplate template)
        {
            return new EmailTemplateResponseDTO
            {
                Id = template.Id,
                Tokens = template.Tokens,
                TemplateName = template.TemplateName,
                EmailSubject = template.EmailSubject,
                EmailBody = template.EmailBody,
                ApplicableFor = template.ApplicableFor,
                ApplicableForName = template.ApplicableFor ?? string.Empty,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            };
        }
    }
}
