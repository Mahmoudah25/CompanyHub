using CompanyHub.Application.Auth.DTOs;
using CompanyHub.Application.Common;
using CompanyHub.Application.TwoFactor.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.TwoFactor
{
    public interface ITwoFactorService
    {
        Task<Result<Enable2FAResponse>> EnableAsync(Guid userId);
        Task<Result> VerifyAsync(Guid userId, Verify2FARequest request);
        bool ValidateCode(string secret, string code); // يستخدم وقت الـ Login كمان
    }
}
