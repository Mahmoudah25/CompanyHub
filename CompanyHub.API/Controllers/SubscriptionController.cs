using CompanyHub.Application.Subscription;
using CompanyHub.Application.Subscription.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscirptionService subscirptionService;
        public SubscriptionController(ISubscirptionService subscirptionService)
        {
            this.subscirptionService = subscirptionService;
        }

        [Authorize(Policy = "Substraction.Create")]
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionRequest request)
        {
            var subscriptionId = await subscirptionService.Subscribe(request);
            return Ok(subscriptionId);
        }

        [Authorize(Policy = "Substraction.Read")]
        [HttpGet]
        public IActionResult current()
        {
            return Ok(subscirptionService.GetCurrent());
        }

        [Authorize(Policy = "Substraction.Update")]
        [HttpDelete("cancel")]
        public async Task<IActionResult> Cancel()
        {
            await subscirptionService.Cancel();
            return NoContent();
        }

        [Authorize(Policy = "Substraction.Update")]
        [HttpPut("Change-Plan")]
        public async Task <IActionResult> Change( ChangePlanRequest request)
        {
            await subscirptionService.Changeplan(request);
            return NoContent();
        }

        [Authorize(Policy = "SuperAdmin")]
        [HttpGet("search")]
        public async Task<IActionResult> Search( SearchSubscriptionRequest request)
        {
            var subscriptions = await subscirptionService.Search(request);
            return Ok(subscriptions);
        }
    }
}
