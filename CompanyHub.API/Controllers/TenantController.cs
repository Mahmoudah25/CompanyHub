using CompanyHub.Application.Tenant;
using CompanyHub.Application.Tenant.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITeanantService teanantService;
        public TenantController(ITeanantService teanantService)
        {
            this.teanantService = teanantService;
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await teanantService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await teanantService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,  UpdateTenantRequest request)
        {
            var result = await teanantService.UpdateAsync(id, request);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpPut("{id}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var result = await teanantService.ActivateAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var result = await teanantService.DeactivateAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [HttpPost("{id}/logo")]  
        public async Task <IActionResult> AddLogo(Guid id,IFormFile file )
        {
            var logo = await teanantService.UploadLogoAsync(id, file);
            return logo.IsSuccess ? Ok(logo) : BadRequest(logo);
        }

        [HttpGet("search")] 
        public async Task<IActionResult> Search([FromQuery] SearchTenantRequest request)
        {
            var result = await teanantService.SearchAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await teanantService.DeleteAsync(id);
            if (result.IsSuccess)
                return Ok(result);
            return NotFound(result);
        }

        [HttpPut("{id}/restore")]
        [Authorize(Policy = "SuperAdmin")]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await teanantService.RestoreAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
