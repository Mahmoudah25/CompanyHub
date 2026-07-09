using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class RolePermission
    {
        // Relations
        public Guid PermissinId { get; set; }
        public Permission Permission { get; set; } = null!;
        public Guid roleId { get; set; }
        public Role role { get; set; } = null!;
    }
}
