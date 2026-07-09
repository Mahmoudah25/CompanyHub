using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class Notification : BaseEntity  
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreateAt { get; set; } 
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        [ForeignKey("Tenants")]
        public Guid tenantId { get; set; }
        public Tenants Tenants { get; set; } = null!;
    }
}
