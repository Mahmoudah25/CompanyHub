using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.User.DTOs
{
    public class AssignRoleToUserRequest
    {
        public List<Guid> RoleIds { get; set; } = new();
    }
}
