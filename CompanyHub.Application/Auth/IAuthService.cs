using CompanyHub.Application.Auth.DTOs;
using CompanyHub.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth
{
    public interface IAuthService
    {
        Task<string> Register(RegisterRequest request);
        Task<AuthResponse> Login(LoginRequest request);
        Task<AuthResponse> Refresh(RefreshTokenRequest refreshToken);
        Task Forgetpassword(ForgetPasswordRequest request);
        Task ResetPassword(ResetPasswordRequest request);
        Task ChangePassword(ChangePasswordRequest request);
        Task LogOut(LogOutRequest request);
        Task<Result> VerifyEmailAsync(VerifyEmailRequest request);
        Task<Result> ResendVerificationAsync(ResendVerificationRequest request);
        Task<Result<List<SessionResponse>>> GetSessionsAsync(Guid userId, string currentRefreshToken);
        Task<Result> RevokeSessionAsync(Guid userId, Guid sessionId);
        Task<Result> RevokeAllOtherSessionsAsync(Guid userId, string currentRefreshToken);
    }
}
