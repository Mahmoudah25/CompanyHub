using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.ApiKey.DTOs
{
    public class CreateApiKeyRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
    }
}
