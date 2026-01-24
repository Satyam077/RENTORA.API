using Microsoft.AspNetCore.Mvc;
using RENTORA.API.Repository.IRepository;

namespace RENTORA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboadController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;

        public AdminDashboadController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalLandlords = await _adminRepository.GetTotalLandlordsAsync();
            var totalAdmins = await _adminRepository.GetTotalAdminsAsync();
            var totalTenants = await _adminRepository.GetTotalTenantsAsync();
            var totalRevenue = await _adminRepository.GetTotalRevenueAsync();

            return Ok(new
            {
                TotalLandlords = totalLandlords,
                TotalAdmins = totalAdmins,
                TotalTenants = totalTenants,
                TotalRevenue = totalRevenue
            });
        }
    }
}
