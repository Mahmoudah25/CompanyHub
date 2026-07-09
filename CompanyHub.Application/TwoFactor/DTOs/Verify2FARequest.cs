using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.TwoFactor.DTOs
{
    public class Verify2FARequest
    {
        public string Code { get; set; } = string.Empty; // 6 digit from app
                                                         
    }
}
