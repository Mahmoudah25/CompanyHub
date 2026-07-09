using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        [ForeignKey("tenant")]
        public Guid tenantId { get; set; }
        public Tenants tenant { get; set; } = null!;
        [ForeignKey("user")]
        public Guid? UserId { get; set; }
        public User user { get; set; } = null!;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } 


    }
}
