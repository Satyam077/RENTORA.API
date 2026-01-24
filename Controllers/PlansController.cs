using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RENTORA.API.Models;
using RENTORA.API.Models.DTOs;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlansController : ControllerBase
    {
        private readonly IPlanRepository _planRepository;

        public PlansController(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _planRepository.GetAllAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] PlanDto planDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var plan = new Plans
            {
                Id = planDto.Id,
                PlanName = planDto.PlanName,
                Description = planDto.Description,
                Price = planDto.Price,
                YearlyDiscount = planDto.YearlyDiscount,
                IsMarkedAsPopular = planDto.IsMarkedAsPopular
            };

            var result = await _planRepository.UpsertAsync(plan, planDto.FeatureIds);

            if (result == null) return NotFound("Plan to update not found.");

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _planRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
