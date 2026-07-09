using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth.DTOs
{
    public class ForgetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
