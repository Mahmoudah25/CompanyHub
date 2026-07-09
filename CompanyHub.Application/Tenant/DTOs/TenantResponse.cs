using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Tenant.DTOs
{
    public class TenantResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SubDomain { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UsersCount { get; set; }
    }
}
