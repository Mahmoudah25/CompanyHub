using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Role.DTOs
{
    public class AssignPermissionsRequest
    {
        public List<Guid> PermissionsIds { get; set; } = new ();
    }
}
