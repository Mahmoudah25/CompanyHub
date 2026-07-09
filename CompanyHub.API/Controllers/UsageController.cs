using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.UsageRecord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsageController : ControllerBase
    {
        private readonly IUsageService usageService;
        private readonly ICurrenttenantService currenttenantService;
        public UsageController(IUsageService usageService,ICurrenttenantService currenttenantService)
        {
            this.usageService = usageService;
            this.currenttenantService = currenttenantService;
        }
      
        [HttpGet]
        public async Task <IActionResult> Get()
        {
            var usage = await usageService.GetUsage();
            return Ok(usage);
        }

        [HttpPost("reconcile")]
        [Authorize(Roles = "SuperAdmin")] // ⚠️ عدّلها حسب نظام الـ Authorization عندك
        public async Task<IActionResult> ReconcileUsage()
        {
            await usageService.ReconcileUsage(currenttenantService.TenantId);
            return Ok(new { message = "Usage reconciled successfully." });
        }

        [HttpPost("reconcile-all")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ReconcileAllUsage()
        {
            await usageService.ReconcileAllUsage();
            return Ok(new { message = "Usage reconciled for all tenants successfully." });
        }
    }
}
