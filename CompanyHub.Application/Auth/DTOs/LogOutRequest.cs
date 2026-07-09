using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth.DTOs
{
    public class LogOutRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
