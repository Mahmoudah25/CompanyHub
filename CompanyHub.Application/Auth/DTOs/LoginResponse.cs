using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth.DTOs
{
    public class LoginResponse
    {
        public bool RequiresTwoFactor { get; set; }
        public string? TempToken { get; set; }   // token مؤقت صالح لدقايق بس، يُستخدم في خطوة الـ 2FA login
        public string? AccessToken { get; set; } // null لو RequiresTwoFactor = true
        public string? RefreshToken { get; set; }
    }
}
