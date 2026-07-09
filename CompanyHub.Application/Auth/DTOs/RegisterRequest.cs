using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth.DTOs
{
    public class RegisterRequest
    {
        public string TenantName { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
