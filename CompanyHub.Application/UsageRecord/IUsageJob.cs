using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.UsageRecord
{
    public interface IUsageJob
    {
        Task RefreshUsage(Guid tenantId);
        Task Execute();
    }
}
