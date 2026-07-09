using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Payment;
using CompanyHub.Application.Payment.DTOs;
using CompanyHub.Infrastructure.Paymob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly IPaymentGateway paymentGateway;
        private readonly IPaymentService paymentService;
        public WebHookController(IPaymentGateway paymentGateway,IPaymentService paymentService)
        {
            this.paymentService = paymentService;
            this.paymentGateway = paymentGateway;
        }

        [AllowAnonymous]
        [HttpPost("paymob")]
        public async Task<IActionResult> PaymobWebhook(
                              [FromBody] PaymobWebhookDto webhook,
                              [FromQuery] string hmac)
        {
            var callbackData = Request.Query
                .ToDictionary(q => q.Key, q => q.Value.ToString());

            // ✅ VerifyHmac في IPaymentGateway مش IPaymentService
            //if (!paymentGateway.VerifyHmac(callbackData, hmac))
            //    return Unauthorized("Invalid HMAC");

            await paymentService.HandleWebhookAsync(webhook);
            return Ok();
        }
    }
}
