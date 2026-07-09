using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name {  get; set; } = string.Empty;
        // Fk
        [ForeignKey("Tenants")]
        public Guid TenantId { get; set; }
        public Tenants Tenants { get; set; } = null!;
        // Realtions
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
