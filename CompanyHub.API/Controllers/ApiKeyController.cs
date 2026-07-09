using CompanyHub.Application.ApiKey;
using CompanyHub.Application.ApiKey.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiKeyController : ControllerBase
    {
        private readonly IApiKeyService keyService;
        public ApiKeyController(IApiKeyService keyService)
        {
            this.keyService = keyService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest request)
        {
            var result = await keyService.CreateAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await keyService.GetAllAsync();
            return Ok(result);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Revoke(Guid id)
        {
            var result = await keyService.RevokeAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
