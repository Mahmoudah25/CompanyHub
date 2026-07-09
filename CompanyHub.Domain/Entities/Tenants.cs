using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class Tenants : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string SubDomain { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string LogoUrl { get; set; } = string.Empty;
        // SoftDelete
        public bool IsDeleted { get; set; } = false; 
        // Realtions
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<UsageRecord> UsageRecords { get; set; } = new List<UsageRecord>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<Notification> Notification { get; set; } = new List<Notification>();
        public ICollection<Subscription> Subscription { get; set; } = new List<Subscription>();
        public ICollection<ApiKey> ApiKey { get; set; } = new List<ApiKey>();   
    }
}
