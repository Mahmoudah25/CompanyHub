using CompanyHub.Application.Aduit;
using CompanyHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Jobs
{
    public class AduitJobs : IAduitJobs
    {
        private readonly IApplicationDBContext context;
        public AduitJobs(IApplicationDBContext context)
        {
            this.context = context;
        }
        public async Task AuditLogCleanJob()
        {
            var cutOff = DateTime.UtcNow.AddDays(-365);
            var expiredUsage = await
                context.auditLogs
                .IgnoreQueryFilters()
                .Where(x => x.CreateAt < cutOff)
                .ToListAsync();
            context.auditLogs.RemoveRange(expiredUsage);
            await context.SaveChangesAsync();
        }
    }
}
