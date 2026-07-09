using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.TwoFactor.DTOs
{
    public class LoginVerify2FARequest
    {
        public string TempToken { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
