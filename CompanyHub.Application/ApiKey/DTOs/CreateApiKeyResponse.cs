using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.ApiKey.DTOs
{
    public class CreateApiKeyresponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PlainKey { get; set; } = string.Empty; 
        public DateTime? ExpiresAt { get; set; }
    }
}
