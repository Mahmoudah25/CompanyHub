using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Subscription;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Jobs
{
    public class SubscriptionJob : ISubscriptionJob
    {
        private readonly IApplicationDBContext context;
        public SubscriptionJob(IApplicationDBContext context)
        {
            this.context = context;
        }
        public async Task ExpireSubscriptions()
        {
            var subscriptions = 
                await context.subscriptions.Where(x=>x.IsActive && x.EndDate < DateTime.UtcNow).ToListAsync();
            foreach (var item in subscriptions)
            {
                item.IsActive = false;
            }
            await context.SaveChangesAsync();
        }

        public async Task PlanLimitAlertJob()
        {
            var subscriptions = await context.subscriptions
                .Include(x => x.Plan)
                .Include(x => x.Tenants)
                .Where(x => x.IsActive)
                .ToListAsync();
            foreach (var sub in subscriptions)
            {
                var usersCount = await context.users
                    .CountAsync(x=>x.TenantId == sub.TenantId);
                var UsagePercent = (double)usersCount / sub.Plan.MaxUsers * 100;   

            }
        }

        public async Task SubscriptionReminder()
        {
            var subscriptions = context.subscriptions.Where(x => x.IsActive && x.EndDate < DateTime.UtcNow.AddDays(3)).ToList();
            foreach (var item in subscriptions)
            {
               context.notification.Add(new CompanyHub.Domain.Entities.Notification
               {
                   Title = "Subscription Reminder",
                   Message = $"Your subscription will expire on {item.EndDate}. Please renew your subscription.",
                   IsRead = false,
                   CreateAt = DateTime.UtcNow,
                   tenantId = item.TenantId
               });
            }
            await context.SaveChangesAsync();
        }
    }
}
