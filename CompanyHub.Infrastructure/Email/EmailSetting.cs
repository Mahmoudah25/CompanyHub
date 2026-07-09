using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Email
{
    public class EmailSetting
    {
        public string host { get; set; } = string.Empty;
        public int port { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSSL { get; set; } 
    }
}
