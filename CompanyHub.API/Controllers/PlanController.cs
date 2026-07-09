using CompanyHub.Application.Plan;
using CompanyHub.Application.Plan.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService planService;
        public PlanController(IPlanService planService)
        {
            this.planService = planService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await planService.GetAllPlans();
            return Ok(plans);
        }

        [HttpGet("{planId}")]
        public async Task<IActionResult> GetPlanById(Guid planId)
        {
            var plan = await planService.GetPlanById(planId);
            if (plan == null)
            {
                return NotFound();
            }
            return Ok(plan);
        }

        [HttpGet("{Name:alpha}")]
        public async Task <IActionResult> GetByName(string Name)
        {
            var plan = await planService.GetPlanByName(Name);
            if (plan == null)
            {
                return NotFound();
            }
            return Ok(plan);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreatePlan([FromBody] CreateplanRequest request)
        {
            var plan = await planService.CreatePlan(request);
            return CreatedAtAction(nameof(GetPlanById), new { PlanName = plan.Name }, plan);
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpPut("{planId}")]
        public async Task<IActionResult> UpdatePlan(Guid planId, [FromBody] UpdatePlanRequest request)
        {
            var result = await planService.UpdatePlan(planId, request);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpDelete("{planId}")]
        public async Task<IActionResult> DeletePlan(Guid planId)
        {
            var result = await planService.DeletePlan(planId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("search")]
        public async Task <IActionResult> Search([FromQuery]SearchPlanRequest request) 
        {
            var plans = await planService.SearchPlanRequests(request);
            return Ok(plans);   
        }
    }
}
