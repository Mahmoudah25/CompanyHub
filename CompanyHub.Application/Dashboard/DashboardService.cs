using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Dashboard.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currentService;
        private readonly ICacheService cacheService;
        private readonly ILogger<DashboardService> logger;
        public DashboardService(IApplicationDBContext context, ICurrenttenantService currentService, ICacheService cacheService,ILogger<DashboardService> logger)
        {
            this.context = context;
            this.currentService = currentService;
            this.cacheService = cacheService;
            this.logger = logger;
        }

       public async Task<DashboardResponsive> GetDashboard()
        {
            var cacheKey = $"Dashboard:{currentService.TenantId}";

            try
            {
                var cachedDashboard = await cacheService.GetAsync<DashboardResponsive>(cacheKey);
                if (cachedDashboard != null)
                    return cachedDashboard;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to read dashboard from cache for tenant {TenantId}, falling back to database.", currentService.TenantId);
            }

            DashboardResponsive dashboard;
            try
            {
                var tenantId = currentService.TenantId;

                var totalUsers = await context.users.CountAsync(x => x.TenantId == tenantId);

                var subscription = await context.subscriptions
                    .Include(x => x.Plan)
                    .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive);

                var roles = await context.roles.CountAsync(x => x.TenantId == tenantId);

                var notification = await context.notification.CountAsync(x => x.tenantId == tenantId);

                var payments = await context.payments.Include(x=>x.SubSubscription).CountAsync(x => x.SubSubscription.TenantId == tenantId);
                var revenue = await context.payments
                    .Include(x => x.SubSubscription)
                    .Where(x => x.SubSubscription.TenantId == tenantId)
                    .SumAsync(x => (decimal?)x.Amount) ?? 0;

                dashboard = new DashboardResponsive
                {
                    TotalUsers = totalUsers,
                    TotalRoles = roles,
                    TotalNotifications = notification,
                    Totalpayments = payments,
                    TotalRevenue = revenue,
                    CurrentPlan = subscription?.Plan.Name ?? "No Plan",
                    SubscriptionActive = subscription != null
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to build dashboard from database for tenant {TenantId}.", currentService.TenantId);
                throw;
            }

            
            try
            {
                await cacheService.SetAsync(cacheKey, dashboard, TimeSpan.FromMinutes(10));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to write dashboard to cache for tenant {TenantId}, continuing without caching.", currentService.TenantId);
            }

            return dashboard;
        }
    }
}
