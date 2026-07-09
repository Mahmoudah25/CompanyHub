using CompanyHub.Application.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        [Authorize(Policy = "Dashboard.Read")]
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await dashboardService.GetDashboard();
            return Ok(result);
        }

    }
}
