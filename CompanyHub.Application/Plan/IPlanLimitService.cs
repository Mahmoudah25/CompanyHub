using CompanyHub.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Plan
{
    public interface IPlanLimitService
    {
        Task<Result> CheckUserLimitAsync(Guid TeanatId);
        Task<Result> CheckRoleLimitAsyunc(Guid TeanatId);

    }
}
