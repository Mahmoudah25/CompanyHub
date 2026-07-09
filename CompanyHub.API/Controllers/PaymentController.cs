using CompanyHub.Application.Payment;
using CompanyHub.Application.Payment.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [Authorize(Policy = "Payment.Create")]
        [HttpPost("initiate")]
        public async Task <IActionResult> InitiatePayment(InitiatePaymentRequest request)
        {
            var res = await  paymentService.InitiatePaymentAsync(request);
            return Ok(res);
        }

        [Authorize(Policy = "Payment.Read")]
        [HttpGet]
        public async Task <IActionResult> Get() 
        {
            var res= await paymentService.GetMyPayments();
            return Ok(res);
        }

        //[Authorize(Policy = "Payment.Create")]
        //[HttpPost]
        //public async Task <IActionResult> Post(CreatepaymentRequest request) 
        //{
        //    var res = await paymentService.CreatePaymentAsync(request);
        //    return Ok(res);
        //}
        
        [Authorize(Policy = "Payment.Update")]
        [HttpPut("{id}/paid")]
        public async Task <IActionResult> MaskAsPaid(Guid id)
        {
             await paymentService.MaskAsAaid(id);
            return NoContent();
        }

        [Authorize(Policy = "Payment.Read")]
        [HttpGet("search")]
        public async Task<IActionResult> Search( SearchPaymentRequest request)
        {
            var res = await paymentService.Search(request);
            return Ok(res);
        }
    }
}
