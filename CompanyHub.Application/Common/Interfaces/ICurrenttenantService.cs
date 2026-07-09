using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Common.Interfaces
{
    public interface ICurrenttenantService
    {
        Guid UserId { get; }
        Guid TenantId { get; }
    }
}
