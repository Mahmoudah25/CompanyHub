using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.UsageRecord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.UsageRecord
{
    public class UsageJobs : IUsageJob
    {
        private readonly IApplicationDBContext context;
        public UsageJobs(IApplicationDBContext context)
        {
            this.context = context;
        }

        public async Task  Execute()
        {
            var tenantIds = await context.tenants
                .Select(t => t.Id)
                   .ToListAsync();

            foreach (var tenantId in tenantIds)
            {
                await RefreshUsage(tenantId);
            }
        }

        public async Task RefreshUsage(Guid tenantId)
        {
            var usersCount = await context.users.CountAsync(x => x.TenantId == tenantId);
            var rolesCount = await context.roles.CountAsync(x => x.TenantId == tenantId);
            var NotificationCount = await context.notification.CountAsync(x => x.tenantId == tenantId);
            var usage = await context.usages.FirstOrDefaultAsync(x => x.TenantsId == tenantId);
            if (usage == null)
            {
                usage = new Domain.Entities.UsageRecord
                {
                    TenantsId = tenantId,
                };
                context.usages.Add(usage);
            }
            usage.NotificatiomCount = NotificationCount;
            usage.UsersCount = usersCount;
            usage.RolesCount = rolesCount;
            usage.LastUpdated = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }
    }
}
