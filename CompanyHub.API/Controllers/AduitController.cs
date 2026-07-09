using CompanyHub.Application.Aduit;
using CompanyHub.Application.Aduit.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AduitController : ControllerBase
    {
        private readonly IAduitService aduitService;
        public AduitController(IAduitService aduitService)
        {
            this.aduitService = aduitService;
        }

        [Authorize(Policy = "Aduit.Read")]
        [HttpGet]
        public async Task<IActionResult> GetAduits()
        {
            var logs = await aduitService.GetLogs();
            return Ok(logs);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() 
        {
            var logs = await aduitService.GetAllAsync();
            return Ok(logs);
        }

        [Authorize(Policy = "Aduit.Read")]
        [HttpGet("search")]
        public async Task <IActionResult> Search( SearchAuditLogRequest request)
        {
            var logs = await aduitService.SearchAsync(request);
            return Ok(logs);
        }
    }
}
