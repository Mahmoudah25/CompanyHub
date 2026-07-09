using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class Subscription : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } 
        //FK
        [ForeignKey("Plan")]
        public Guid PlanId { get; set; }
        public Plan Plan { get; set; } = null!;
        [ForeignKey("Tenants")]
        public Guid TenantId { get; set; }
        public Tenants Tenants { get; set; } = null!;
        ///Relations
        public ICollection<Payment> payments { get; set; } = new List<Payment>();
    }
}
