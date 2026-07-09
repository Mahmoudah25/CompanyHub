using CompanyHub.Application.Auth.DTOs;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.TwoFactor.DTOs;
using CompanyHub.Application.TwoFactor;
using CompanyHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompanyHub.Application.Common;
using Microsoft.Extensions.Logging;
using CompanyHub.Application.Notification;
using Microsoft.AspNetCore.Http;


namespace CompanyHub.Application.Auth
{
    public class AuthService : IRefreshToken,IAuthService
    {
        private readonly IApplicationDBContext context;
        private readonly IJwtProvider provider;
        private readonly IRefreshTokenProvider refreshTokenProvider;
        private readonly IEmailService emailService;
        private readonly ICurrenttenantService CurrentService;
        private readonly ITwoFactorService twoFactorService;
        private readonly ILogger<AuthService> logger; 
        private readonly INotificationSender notificationSender;
        private readonly IHttpContextAccessor httpContextAccessor;
        public AuthService
            (IJwtProvider provider, IApplicationDBContext context, 
            IRefreshTokenProvider refreshTokenProvider, IEmailService emailService,
            ICurrenttenantService currentService, ITwoFactorService twoFactorService,
            ILogger<AuthService> logger,INotificationSender notificationSender,
            IHttpContextAccessor httpContextAccessor)
        {
            this.provider = provider;
            this.context = context;
            this.refreshTokenProvider = refreshTokenProvider;
            this.emailService = emailService;
            CurrentService = currentService;
            this.twoFactorService = twoFactorService;
            this.logger = logger;
            this.notificationSender = notificationSender;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> Register(RegisterRequest request)
        {
            var tenant = new Tenants
            {
                Name = request.TenantName,
            };
            context.tenants.Add(tenant);
            await context.SaveChangesAsync();

            var user = new CompanyHub.Domain.Entities.User
            {
                Email = request.AdminEmail,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                TenantId = tenant.Id
            };

            var verificationToken = Guid.NewGuid().ToString("N");
            user.EmailVerificationToken = verificationToken;
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
            user.EmailVerified = false;

            context.users.Add(user);
            await context.SaveChangesAsync();

            var adminRole = new CompanyHub.Domain.Entities.Role
            {
                Name = "Admin",
                TenantId = tenant.Id
            };
            context.roles.Add(adminRole);
            await context.SaveChangesAsync();
            var allPermissions = await context.permissions.ToListAsync();
            foreach (var permission in allPermissions)
            {
                context.rolePermissions.Add(new RolePermission
                {
                    roleId = adminRole.Id,
                    PermissinId = permission.Id
                });
            }

            // 🆕 اربط اليوزر بالـ Admin Role
            context.userRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id
            });

            await context.SaveChangesAsync();
            var verificationLink = $"http://localhost:5048/api/Auth/VerifyEmail?token={verificationToken}";

            // إيميل حقيقي واحد بس، فيه الـ Token الصحيح
            await emailService.SendEmail(
                request.AdminEmail,
                "Welcome to CompanyHub - Verify your email",
                $"<h1>Welcome to CompanyHub</h1>" +
                $"<p>Your account has been created successfully.</p>" +
                $"<p>Please verify your email by clicking the link below:</p>" +
                $"<a href='{verificationLink}'>Verify Email</a>" +
                $"<p>Or use this token directly: <b>{verificationToken}</b></p>" +
                $"<p>This link expires in 24 hours.</p>");

            return "Registration successful. Please check your email to verify your account before logging in.";
        }
        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var deviceInfo = httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var user = await context.users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == request.AdminEmail);

            if (!user.EmailVerified)
                throw new UnauthorizedAccessException("Please verify your email before logging in.");

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isValidPassword)
                throw new UnauthorizedAccessException("Wrong password.");

            // لو مفعّل 2FA، نوقف هنا ونرجع TempToken بس - من غير AccessToken ولا RefreshToken
            if (user.TwoFactorEnabled)
            {
                var tempToken = provider.GenerateTempToken(user); // شرح تحت
                return new AuthResponse
                {
                    RequiresTwoFactor = true,
                    TempToken = tempToken
                };
            }

            var roles = await context.userRoles
                .IgnoreQueryFilters()
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.role)
                .Select(ur => ur.role.Name)
                .ToListAsync();

            var permissions = await context.userRoles
                .IgnoreQueryFilters()
                .Where(u => u.UserId == user.Id)
                .SelectMany(u => u.role.RolePermissions)
                .Select(p => p.Permission.Name)
                .Distinct()
                .ToListAsync();

            var token = provider.GenerateToken(user, roles, permissions);
            var refreshToken = refreshTokenProvider.Generate();

            var refreshEntity = new RefreshToken
            {
                token = refreshToken,
                UserId = user.Id,
                Expireddate = DateTime.UtcNow.AddDays(30),
                DeviceInfo = deviceInfo,
                IpAddress = ipAddress,
                CreateAt = DateTime.UtcNow,
                IsRevoked = false

            };
            context.refreshTokens.Add(refreshEntity);
            await context.SaveChangesAsync();

            return new AuthResponse
            {
                RequiresTwoFactor = false,
                AccessToken = token,
                RefreshToken = refreshToken
            };
        }
        public async Task<AuthResponse> LoginVerify2FA(LoginVerify2FARequest request)
        {
            // 1. فك التوكن المؤقت وطلع منه الـ UserId
            var userId = provider.ValidateTempTokenAndGetUserId(request.TempToken);
            if (userId is null)
                throw new UnauthorizedAccessException("Invalid or expired temp token.");

            var user = await context.users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null)
                throw new KeyNotFoundException("User not found.");

            if (string.IsNullOrEmpty(user.TwoFactorSecret))
                throw new UnauthorizedAccessException("2FA is not configured for this user.");

            // 2. تحقق من الكود
            var isValidCode = twoFactorService.ValidateCode(user.TwoFactorSecret, request.Code);
            if (!isValidCode)
                throw new UnauthorizedAccessException("Invalid 2FA code.");

            // 3. دلوقتي بس نولّد التوكنز الحقيقية (نفس منطق Login العادي)
            var roles = await context.userRoles
                .IgnoreQueryFilters()
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.role)
                .Select(ur => ur.role.Name)
                .ToListAsync();

            var permissions = await context.userRoles
                .IgnoreQueryFilters()
                .Where(u => u.UserId == user.Id)
                .SelectMany(u => u.role.RolePermissions)
                .Select(p => p.Permission.Name)
                .Distinct()
                .ToListAsync();

            var token = provider.GenerateToken(user, roles, permissions);
            var refreshToken = refreshTokenProvider.Generate();

            var refreshEntity = new CompanyHub.Domain.Entities.RefreshToken
            {
                token = refreshToken,
                UserId = user.Id,
                Expireddate = DateTime.UtcNow.AddDays(30)
            };
            context.refreshTokens.Add(refreshEntity);
            await context.SaveChangesAsync();

            return new AuthResponse
            {
                RequiresTwoFactor = false,
                AccessToken = token,
                RefreshToken = refreshToken
            };
        }
        public async Task<AuthResponse> Refresh(RefreshTokenRequest refreshToken)
        {
            var storedToken =
                await context.refreshTokens
                //.Include(u=>u.user)
                .FirstOrDefaultAsync(x => x.token == refreshToken.RefreshToken);
            if (storedToken == null)
                throw new Exception(" Invalid Token");
            if (storedToken.IsRevoked)
                throw new Exception(" Invalid token Revoke");
            if (storedToken.Expireddate < DateTime.UtcNow)
                throw new Exception(" Refresh token Expired ");

            var roles =
                await context.userRoles.Where(o => o.UserId == storedToken.UserId)
                .Select(o => o.role.Name)
                .ToListAsync();
            var permissions =
                await context.userRoles.Where(x => x.UserId == storedToken.UserId)
                .SelectMany(x => x.role.RolePermissions)
                .Select(x => x.role.Name)
                .Distinct()
                .ToListAsync();

            var accessToken =
                provider.GenerateToken(storedToken.users, roles, permissions);

            storedToken.IsRevoked = true;
            storedToken.RevokeTime = DateTime.UtcNow;

            var newRefreshToken = refreshTokenProvider.Generate();
            context.refreshTokens.Add(new CompanyHub.Domain.Entities.RefreshToken
            {
                token = newRefreshToken,
                UserId = storedToken.UserId,
                Expireddate = DateTime.UtcNow.AddDays(30)
            });

            await context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,

            };

        }

        public async Task Forgetpassword(ForgetPasswordRequest request)
        {
            var user = await context.users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return;
            var token = provider.GenerateToken(user, new List<string>(), new List<string>());
            //var resetToekn = refreshTokenProvider.Generate();
            var resetToekn =new CompanyHub.Domain.Entities.PasswordRefreshToken
            {
                UserId = user.Id,
                Token = token,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            };
            context.passwordRefreshToken.Add(resetToekn);
            await context.SaveChangesAsync();
            var restetLink = $"http://localhost:5048/ResetPassword?token{token}";
            await
                emailService.SendEmail
                (request.Email, "Reset Password",
                $"<h2> Password Reset </h2>  <p> Clcik the Link below to reset your password </p> <a href={restetLink}> Reset Password </a> ");
        }

        public async Task ResetPassword(ResetPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new Exception(" Passwords dont Match");
            var resetToken = await context.passwordRefreshToken
                .Include(x=>x.User)
                .FirstOrDefaultAsync(x=> x.Token == request.Token);
            if(resetToken == null)
                throw new Exception(" Invalid Token");
            if (resetToken.IsUsed)
                throw new Exception(" Token Already Used");
            if (resetToken.ExpiredAt < DateTime.UtcNow)
                throw new Exception(" Token Expired");
            resetToken.User.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            resetToken.IsUsed = true;
            await context.SaveChangesAsync();
        }

        public async Task ChangePassword(ChangePasswordRequest request)
        {
            var user = context.users.FirstOrDefault(u => u.Id == CurrentService.UserId );
            if (user == null)
                throw new Exception("User not found.");
            var isValidPassword = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);
            if (!isValidPassword)
                throw new Exception("Wrong current password.");
            if(request.NewPassword != request.ConfirmPassword)
                throw new Exception("New password and confirm password do not match.");
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await context.SaveChangesAsync();
        }

        public async Task LogOut(LogOutRequest request)
        {
            var refresToken = await context.refreshTokens
                .FirstOrDefaultAsync(x=>x.token == request.RefreshToken && x.UserId == CurrentService.UserId);
            if(refresToken == null)
                throw new Exception(" Invalid Token");
            refresToken.IsRevoked = true;
            await context.SaveChangesAsync();
        }

        public async Task<Result> VerifyEmailAsync(VerifyEmailRequest request)
        {
            try
            {
                var user = await context.users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.EmailVerificationToken == request.Token);
                if(user == null)
                    return Result.Failure("Invalid token.");
                if(user.EmailVerified)
                    return Result.Failure("Email already verified.");
                if(user.EmailVerificationTokenExpiry < DateTime.UtcNow || user.EmailVerificationTokenExpiry is null)
                    return Result.Failure("Token expired.");
                user.EmailVerified = true;
                user.EmailVerificationToken = null;
                user.EmailVerificationTokenExpiry = null;
                await context.SaveChangesAsync();
                return Result.Success();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error verifying email.");
                return Result.Failure(" An Error While Verification");
            }
        }

        public async Task<Result> ResendVerificationAsync(ResendVerificationRequest request)
        {
            try
            {
                var user = await context.users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                    return Result.Success(); // until email not found
                if (user.EmailVerified)
                    return Result.Success(); // until email already verified
                var token =Guid.NewGuid().ToString("N"); // random string
                user.EmailVerificationToken = token;
                user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
                await context.SaveChangesAsync();
                var verificationLink = $"http://localhost:5048/VerifyEmail?token={token}";
                await notificationSender.SendAsync(
                    user.Id,
                    "Email Verification",
                    $"<h2>Email Verification</h2><p>Click the link below to verify your email:</p><a href='{verificationLink}'>Verify Email</a>"
                );
                return Result.Success();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error resending verification email.");
                return Result.Failure("An error occurred while resending verification email.");
            }
        }

        public async Task<Result<List<SessionResponse>>> GetSessionsAsync(Guid userId, string currentRefreshToken)
        {
            try
            {
                var session = await context.refreshTokens
                    .Where(x => x.UserId == userId && !x.IsRevoked && x.Expireddate > DateTime.UtcNow)
                    .OrderByDescending(x => x.CreateAt)
                    .Select(x => new SessionResponse
                    {
                        Id = x.Id,
                        DeviceInfo = x.DeviceInfo,
                        IpAddress = x.IpAddress,
                        CreatedAt = x.CreateAt,
                        ExpiresAt = x.Expireddate,
                        IsCurrentSession = x.token == currentRefreshToken
                    })
                    .ToListAsync();
                return Result<List<SessionResponse>>.Success(session);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error retrieving sessions.");
                return Result<List<SessionResponse>>.Failure("An error occurred while retrieving sessions.");
            }
        }

        public async Task<Result> RevokeSessionAsync(Guid userId, Guid sessionId)
        {
            try
            {
                var session = await context.refreshTokens
                                   .FirstOrDefaultAsync(x => x.Id == sessionId && x.UserId == userId);
                if (session == null)
                    return Result.Failure("Session not found.");
                session.IsRevoked = true;
                session.RevokeTime = DateTime.UtcNow;
                await context.SaveChangesAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error revoking session.");
                return Result.Failure("An error occurred while revoking the session.");
            }
        }

        public async Task<Result> RevokeAllOtherSessionsAsync(Guid userId, string currentRefreshToken)
        {
            var session = await context.refreshTokens
                .Where(x => x.UserId == userId && x.token != currentRefreshToken && !x.IsRevoked)
                .ToListAsync();
            foreach (var item in session)
            {
                item.IsRevoked = true;
                item.RevokeTime = DateTime.UtcNow;
            }
            await context.SaveChangesAsync();
            return Result.Success();
        }
    }
}
