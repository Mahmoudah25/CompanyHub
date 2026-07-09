using CompanyHub.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEmailService emailService;
        public TestController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        [HttpPost("Test-Email")]
        public async Task <IActionResult> TestEmail(IEmailService email)
        {
            await email.SendEmail("reasds@gmail","Wlcom", "<h1>Hello</h1>");
            return Ok("Email Sent");
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok("Authanticated");
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("Admin")]
        public IActionResult Admin() 
        {
            return Ok(" Welcome Admin ");
        }
        [Authorize(Policy = "User.Read")]
        [HttpGet]
        public IActionResult UserRead()
        {
            return Ok(" Welcome User with Read Permission ");
        }
    }
}
