using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class ApiKey : BaseEntity
    {
        [ForeignKey("Tenants")]
        public Guid tenantId { get; set; }
        public Tenants Tenants { get; set; } = null!;
        public string Name { get; set; } = string.Empty;      
        public string KeyHash { get; set; } = string.Empty;   
        public string Prefix { get; set; } = string.Empty;   
        public bool IsActive { get; set; } = true;
        public DateTime? LastUsedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
