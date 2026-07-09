using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Tenant.DTOs
{
    public class UpdateTenantRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? SubDomain { get; set; }
        public string? LogoUrl { get; set; }
    }
}
