using CompanyHub.Application.Report;
using CompanyHub.Application.Report.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService reportsService;
        public ReportController(IReportService reportsService)
        {
            this.reportsService = reportsService;
        }

        [Authorize(Policy = "Report.Read")]
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] RevenueReportRequest request)
        {
            var result = await reportsService.GetRevenueReportAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "Report.Read")]
        [HttpGet("subscriptions-overview")]
        public async Task<IActionResult> GetSubscriptionsOverview()
        {
            var result = await reportsService.GetSubscriptionsOverviewAsync();
            return Ok(result);
        }
    }
}
