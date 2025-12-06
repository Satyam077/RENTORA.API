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
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(
            IPropertyRepository propertyRepository,
            ILogger<PropertyController> logger)
        {
            _propertyRepository = propertyRepository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseModel>> GetAllProperties()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var properties = await _propertyRepository.GetAllAsync();
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Properties retrieved successfully";
                response.data = properties;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving properties");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving properties";
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> GetPropertyById(string id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var property = await _propertyRepository.GetByIdAsync(id);

                if (property == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Property with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Property retrieved successfully";
                response.data = property;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving property with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving the property";
                return StatusCode(500, response);
            }
        }

        [HttpGet("owner/{ownerId}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseModel>> GetPropertiesByOwnerId(string ownerId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var properties = await _propertyRepository.GetByOwnerIdAsync(ownerId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Properties retrieved successfully";
                response.data = properties;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving properties for owner: {OwnerId}", ownerId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving properties";
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> CreateProperty([FromBody] PropertyCreateDTO createDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid property data";
                    response.data = ModelState;
                    return BadRequest(response);
                }

                // Check if property name already exists for this owner
                var exists = await _propertyRepository.ExistsAsync(createDTO.PropertyName, createDTO.OwnerId);
                if (exists)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = $"Property with name '{createDTO.PropertyName}' already exists for this owner";
                    return BadRequest(response);
                }

                // Map DTO to PropertyModel
                var property = new PropertyModel
                {
                    OwnerId = createDTO.OwnerId,
                    PropertyName = createDTO.PropertyName,
                    Description = createDTO.Description,
                    Type = createDTO.Type,
                    Address = createDTO.Address,
                    //Units = createDTO.Units ?? new List<UnitModel>(),
                    Images = createDTO.Images ?? new List<string>(),
                    Documents = createDTO.Documents ?? new List<string>(),
                    DefaultRentAmount = createDTO.DefaultRentAmount,
                    DefaultDueDay = createDTO.DefaultDueDay,
                    Notes = createDTO.Notes,
                    CreatedBy = createDTO.CreatedBy
                };

                var createdProperty = await _propertyRepository.CreateAsync(property);

                response.Success = true;
                response.Status = StatusCodes.Status201Created;
                response.Message = CommonMessage.MessageSuccess.IsPropertySaved;
                response.data = createdProperty;

                return CreatedAtAction(
                    nameof(GetPropertyById),
                    new { id = createdProperty.Id },
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating property");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while creating the property";
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseModel>> UpdateProperty(string id, [FromBody] PropertyUpdateDTO updateDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid property data";
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

                var existingProperty = await _propertyRepository.GetByIdAsync(id);
                if (existingProperty == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Property with ID '{id}' not found";
                    return NotFound(response);
                }

                // Map DTO to PropertyModel
                var property = new PropertyModel
                {
                    Id = updateDTO.Id,
                    OwnerId = updateDTO.OwnerId,
                    PropertyName = updateDTO.PropertyName,
                    Description = updateDTO.Description,
                    Type = updateDTO.Type,
                    Address = updateDTO.Address,
                    //Units = updateDTO.Units ?? new List<UnitModel>(),
                    Images = updateDTO.Images ?? new List<string>(),
                    Documents = updateDTO.Documents ?? new List<string>(),
                    DefaultRentAmount = updateDTO.DefaultRentAmount,
                    DefaultDueDay = updateDTO.DefaultDueDay,
                    Notes = updateDTO.Notes,
                    IsActive = updateDTO.IsActive,
                    UpdatedBy = updateDTO.UpdatedBy,
                    CreatedBy = existingProperty.CreatedBy,
                    CreatedAt = existingProperty.CreatedAt
                };

                var updatedProperty = await _propertyRepository.UpdateAsync(property);

                if (updatedProperty == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Property with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = CommonMessage.MessageSuccess.IsPropertySaved;
                response.data = updatedProperty;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating property with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while updating the property";
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseModel>> DeleteProperty(string id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var result = await _propertyRepository.DeleteAsync(id);

                if (!result)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Property with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Property deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting property with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while deleting the property";
                return StatusCode(500, response);
            }
        }
    }
}
