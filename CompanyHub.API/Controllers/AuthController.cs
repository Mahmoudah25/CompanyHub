using CompanyHub.Application.Auth;
using CompanyHub.Application.Auth.DTOs;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.TwoFactor;
using CompanyHub.Application.TwoFactor.DTOs;
using CompanyHub.Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService authService;
        private readonly ITwoFactorService twoFactorService;
        private readonly ICurrenttenantService currenttenantService;
        private readonly IUserService userService;
        public AuthController(AuthService authService,ITwoFactorService twoFactor, ICurrenttenantService currenttenantService)
        {
            this.authService = authService;
            this.twoFactorService = twoFactor;
            this.currenttenantService = currenttenantService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> reister(RegisterRequest request)
        {
            var token = await authService.Register(request);
            return Ok(new
            {
                token
            });
        }

        //LogIn
        [HttpPost("Login")]
        public async Task<IActionResult> login(LoginRequest request)
        {
            var res = await authService.Login(request);
            if (res == null)
                return BadRequest();
            return Ok(res);
        }

        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var result = await authService.VerifyEmailAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("Resend-Verification")]
        public async Task <IActionResult> ResendVerification(ResendVerificationRequest request)
        {
            var result = await authService.ResendVerificationAsync(request);
            return Ok(result);
        }

        [HttpPost("2fa/enable")]
        [Authorize]
        public async Task<IActionResult> Enable2FA()
        {
            
            var result = await twoFactorService.EnableAsync(currenttenantService.UserId);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpPost("2fa/verify")]
        [Authorize]
        public async Task<IActionResult> Verify2FA(Verify2FARequest request)
        {
            var result = await twoFactorService.VerifyAsync(currenttenantService.UserId, request);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }

        //refresh Token
        [HttpPost("Refresh")]
        public async Task <IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var res = await authService.Refresh(request);
            return Ok(res);

        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
        {
            await authService.Forgetpassword(request);
            return Ok(new 
            {
                Message = "Password reset link sent to your email." 
            });
        }

        [HttpPost("ResetPassword")]
        public async Task <IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            await authService.ResetPassword(request);
            return Ok(new
            {
                Message = "Password reset successfully."
            });
        }
        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task <IActionResult> ChangePassword(ChangePasswordRequest request)
        {
             await authService.ChangePassword(request);
            return Ok(new
            {
                Message = "Password changed successfully."
            });
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(LogOutRequest request)
        {
            await authService.LogOut(request);
            return Ok(new
            {
                Message = "Logged out successfully."
            });
        }

        [Authorize]
        [HttpGet("Sessions")]
        public async Task<IActionResult> GetSessions([FromHeader(Name = "X-Refresh-Token")] string? refreshToken)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                      ?? Guid.Empty.ToString());

            var result = await authService.GetSessionsAsync(userId, refreshToken ?? string.Empty); // ✅ استخدم الـ parameter

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpDelete("Sessions/{sessionId}")]
        public async Task <IActionResult> RevokeSession(Guid sessionId)
        {
            var result = await authService.RevokeSessionAsync(currenttenantService.UserId, sessionId);
            return Ok(result);
        }


        [HttpDelete("sessions/revoke-others")]
        [Authorize]
        public async Task<IActionResult> RevokeOtherSessions()
        {
            var currentRefreshToken = Request.Cookies["refreshToken"] ?? string.Empty;
            var result = await authService.RevokeAllOtherSessionsAsync(currenttenantService.UserId, currentRefreshToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // Me 
        [Authorize]
        [HttpGet("Me")]
        public IActionResult me([FromServices] ICurrenttenantService service)
        {
            return Ok(new
            {
                UserId = service.UserId,
                TenantId = service.TenantId
            });
        }

    }
}
