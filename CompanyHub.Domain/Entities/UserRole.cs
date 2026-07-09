using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class UserRole
    {
  
        public Guid UserId { get; set; }
        public User user { get; set; } = null!;
        public Guid RoleId { get; set; }
        public Role role { get; set; } = null!;
    }
}
