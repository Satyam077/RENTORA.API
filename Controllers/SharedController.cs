using Microsoft.AspNetCore.Mvc;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedController : ControllerBase
    {
        private readonly IPlanRepository _planRepository;
        public SharedController(IPlanRepository planRepository)
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
    }
}
