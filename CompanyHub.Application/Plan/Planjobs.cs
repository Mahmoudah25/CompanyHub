using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Notification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Plan
{
    public class Planjobs : IPlanJobs
    {
        private readonly IApplicationDBContext context;
        private readonly INotificationSender notificationSender;
        public Planjobs(IApplicationDBContext context,INotificationSender notificationSender)
        {
            this.context = context;
            this.notificationSender = notificationSender;
        }
        public async Task PlanLimitAlertJob()
        {
            var subscription = await context.subscriptions.
                Include(x => x.Plan)
                .Include(x => x.Tenants)
                .Where(x => x.IsActive)
                .ToListAsync();
            foreach (var sub in subscription)
            {
                var usersCount = await context.users
                    .CountAsync(x => x.TenantId == sub.TenantId);
                var UsagePercent = (double)usersCount / sub.Plan.MaxUsers * 100;
                if (UsagePercent >= 90)
                {
                    await notificationSender.SendAsync(sub.TenantId, "Plan Limit Alert", $"You have used {UsagePercent}% of your plan limit. Please upgrade your plan to avoid any service disruption.");
                }
            }
        }
    }
}
