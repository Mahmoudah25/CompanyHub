using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        // Realtions
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
