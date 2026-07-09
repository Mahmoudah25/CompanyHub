using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class UsageRecord : BaseEntity
    {
        [ForeignKey("Tenants")]
        public Guid TenantsId {  get; set; }
        public Tenants Tenants { get; set; } = null!;
        public int UsersCount { get; set; } 
        public int RolesCount { get; set; }
        public int NotificatiomCount { set; get; }
        public int ApiCallsCount { get; set; }  
        public decimal StorageUsedGb { get; set; }
        public DateTime LastUpdated {  get; set; }
    }
}
