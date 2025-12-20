using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RENTORA.API.Models;
using RENTORA.API.Models.Enums;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceRepositoy _maintenanceRepository;

        public MaintenanceController(IMaintenanceRepositoy maintenanceRepository)
        {
            _maintenanceRepository = maintenanceRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var maintenanceRequests = await _maintenanceRepository.GetAllAsync();
                return Ok(maintenanceRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving maintenance requests", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var maintenance = await _maintenanceRepository.GetByIdAsync(id);
                if (maintenance == null)
                    return NotFound(new { message = "Maintenance request not found" });

                return Ok(maintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving maintenance request", error = ex.Message });
            }
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetByTenantId(string tenantId)
        {
            try
            {
                var maintenanceRequests = await _maintenanceRepository.GetByTenantIdAsync(tenantId);
                return Ok(maintenanceRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving tenant maintenance requests", error = ex.Message });
            }
        }

        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetByPropertyId(string propertyId)
        {
            try
            {
                var maintenanceRequests = await _maintenanceRepository.GetByPropertyIdAsync(propertyId);
                return Ok(maintenanceRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving property maintenance requests", error = ex.Message });
            }
        }

        [HttpGet("unit/{unitId}")]
        public async Task<IActionResult> GetByUnitId(string unitId)
        {
            try
            {
                var maintenanceRequests = await _maintenanceRepository.GetByUnitIdAsync(unitId);
                return Ok(maintenanceRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving unit maintenance requests", error = ex.Message });
            }
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetByOwnerId(string ownerId)
        {
            try
            {
                var maintenanceRequests = await _maintenanceRepository.GetByOwnerIdAsync(ownerId);
                return Ok(maintenanceRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving owner maintenance requests", error = ex.Message });
            }
        }

        [HttpGet("landlord/{landlordId}")]
        public async Task<IActionResult> GetByLandlordId(string landlordId)
        {
            try
            {
                var maintenanceRequests = await _maintenanceRepository.GetByLandlordIdAsync(landlordId);
                return Ok(maintenanceRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving landlord maintenance requests", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Maintenance maintenance)
        {
            try
            {
                if (maintenance == null)
                {
                    return BadRequest(new { message = "Maintenance request data is required" });
                }

                if (string.IsNullOrEmpty(maintenance.TenantId) || 
                    string.IsNullOrEmpty(maintenance.Category) || 
                    string.IsNullOrEmpty(maintenance.Title))
                {
                    return BadRequest(new { 
                        message = "TenantId, Category, and Title are required",
                        tenantId = maintenance.TenantId,
                        category = maintenance.Category,
                        title = maintenance.Title
                    });
                }

                var createdMaintenance = await _maintenanceRepository.CreateAsync(maintenance);
                return CreatedAtAction(nameof(GetById), new { id = createdMaintenance.Id }, createdMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating maintenance request", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Maintenance maintenance)
        {
            try
            {
                if (id != maintenance.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var updatedMaintenance = await _maintenanceRepository.UpdateAsync(maintenance);
                if (updatedMaintenance == null)
                    return NotFound(new { message = "Maintenance request not found" });

                return Ok(updatedMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating maintenance request", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _maintenanceRepository.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Maintenance request not found" });

                return Ok(new { message = "Maintenance request deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting maintenance request", error = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] Status status)
        {
            try
            {
                var updatedMaintenance = await _maintenanceRepository.UpdateStatusAsync(id, status);
                if (updatedMaintenance == null)
                    return NotFound(new { message = "Maintenance request not found" });

                return Ok(updatedMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating maintenance status", error = ex.Message });
            }
        }

        [HttpPatch("{id}/schedule")]
        public async Task<IActionResult> ScheduleMaintenance(string id, [FromBody] DateTime scheduledDate)
        {
            try
            {
                var updatedMaintenance = await _maintenanceRepository.ScheduleMaintenanceAsync(id, scheduledDate);
                if (updatedMaintenance == null)
                    return NotFound(new { message = "Maintenance request not found" });

                return Ok(updatedMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error scheduling maintenance", error = ex.Message });
            }
        }

        [HttpPatch("{id}/rate")]
        public async Task<IActionResult> RateMaintenance(string id, [FromBody] double rating)
        {
            try
            {
                if (rating < 1 || rating > 5)
                    return BadRequest(new { message = "Rating must be between 1 and 5" });

                var updatedMaintenance = await _maintenanceRepository.RateMaintenanceAsync(id, rating);
                if (updatedMaintenance == null)
                    return NotFound(new { message = "Maintenance request not found" });

                return Ok(updatedMaintenance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error rating maintenance", error = ex.Message });
            }
        }
    }
}

