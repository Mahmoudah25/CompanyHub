using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth.DTOs
{
    public class VerifyEmailRequest
    {
        public string Token { get; set; } = string.Empty;   
    }
}
