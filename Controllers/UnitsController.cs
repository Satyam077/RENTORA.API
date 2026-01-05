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
    public class UnitsController : ControllerBase
    {
        private readonly IUnitsRepository _unitsRepository;
        private readonly ILogger<UnitsController> _logger;

        public UnitsController(
            IUnitsRepository unitsRepository,
            ILogger<UnitsController> logger)
        {
            _unitsRepository = unitsRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel>> GetAllUnits()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var units = await _unitsRepository.GetAllAsync();
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Units retrieved successfully";
                response.data = units;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving units");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving units";
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel>> GetUnitById(string id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var unit = await _unitsRepository.GetByIdAsync(id);

                if (unit == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Unit with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Unit retrieved successfully";
                response.data = unit;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unit with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving the unit";
                return StatusCode(500, response);
            }
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<ResponseModel>> GetUnitsByPropertyId(string propertyId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var units = await _unitsRepository.GetByPropertyIdAsync(propertyId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Units retrieved successfully";
                response.data = units;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving units for property: {PropertyId}", propertyId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving units";
                return StatusCode(500, response);
            }
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<ActionResult<ResponseModel>> GetUnitsByOwnerId(string ownerId)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var units = await _unitsRepository.GetByOwnerIdAsync(ownerId);
                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Units retrieved successfully";
                response.data = units;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving units for owner: {OwnerId}", ownerId);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while retrieving units";
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseModel>> CreateUnit([FromBody] UnitCreateDTO createDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid unit data";
                    response.data = ModelState;
                    return BadRequest(response);
                }

                // Check if unit name already exists for this property
                var exists = await _unitsRepository.ExistsAsync(createDTO.UnitName, createDTO.PropertyId);
                if (exists)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = $"Unit with name '{createDTO.UnitName}' already exists for this property";
                    return BadRequest(response);
                }

                // Map DTO to UnitModel
                var unit = new UnitModel
                {
                    OwnerId = createDTO.OwnerId,
                    PropertyId = createDTO.PropertyId,
                    TenantId = createDTO.TenantId,
                    UnitName = createDTO.UnitName,
                    RentAmount = createDTO.RentAmount,
                    SecurityDeposit = createDTO.SecurityDeposit,
                    IsOccupied = createDTO.IsOccupied,
                    DueDay = createDTO.DueDay,
                    Notes = createDTO.Notes,
                    CreatedBy = createDTO.CreatedBy
                };

                var createdUnit = await _unitsRepository.CreateAsync(unit);

                response.Success = true;
                response.Status = StatusCodes.Status201Created;
                response.Message = "Unit created successfully";
                response.data = createdUnit;

                return CreatedAtAction(
                    nameof(GetUnitById),
                    new { id = createdUnit.Id },
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating unit");
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while creating the unit";
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel>> UpdateUnit(string id, [FromBody] UnitUpdateDTO updateDTO)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid unit data";
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

                var existingUnit = await _unitsRepository.GetByIdAsync(id);
                if (existingUnit == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Unit with ID '{id}' not found";
                    return NotFound(response);
                }

                // Map DTO to UnitModel
                var unit = new UnitModel
                {
                    Id = updateDTO.Id,
                    OwnerId = updateDTO.OwnerId,
                    PropertyId = updateDTO.PropertyId,
                    TenantId = updateDTO.TenantId,
                    UnitName = updateDTO.UnitName,
                    RentAmount = updateDTO.RentAmount,
                    SecurityDeposit = updateDTO.SecurityDeposit,
                    IsOccupied = updateDTO.IsOccupied,
                    DueDay = updateDTO.DueDay,
                    Notes = updateDTO.Notes,
                    IsActive = updateDTO.IsActive,
                    UpdatedBy = updateDTO.UpdatedBy,
                    CreatedBy = existingUnit.CreatedBy,
                    CreatedAt = existingUnit.CreatedAt
                };

                var updatedUnit = await _unitsRepository.UpdateAsync(unit);

                if (updatedUnit == null)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Unit with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Unit updated successfully";
                response.data = updatedUnit;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating unit with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while updating the unit";
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel>> DeleteUnit(string id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                var result = await _unitsRepository.DeleteAsync(id);

                if (!result)
                {
                    response.Success = false;
                    response.Status = StatusCodes.Status404NotFound;
                    response.Message = $"Unit with ID '{id}' not found";
                    return NotFound(response);
                }

                response.Success = true;
                response.Status = StatusCodes.Status200OK;
                response.Message = "Unit deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting unit with ID: {Id}", id);
                response.Success = false;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = "An error occurred while deleting the unit";
                return StatusCode(500, response);
            }
        }
    }
}
