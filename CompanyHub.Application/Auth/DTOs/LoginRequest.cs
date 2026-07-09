using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth.DTOs
{
    public class LoginRequest
    {
        public string AdminEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
