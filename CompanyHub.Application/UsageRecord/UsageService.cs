using CompanyHub.Application.Auth;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.UsageRecord.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.UsageRecord
{
    public class UsageService : IUsageService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currentService;
        private readonly ICacheService cacheService;
        private readonly ILogger<UsageService> logger;

        public UsageService(IApplicationDBContext context, ICurrenttenantService currentService,ICacheService cacheService, ILogger<UsageService> logger)
        {
            this.context = context;
            this.currentService = currentService;
            this.cacheService = cacheService;
            this.logger = logger;
        }
        public async Task<UsageResponse> GetUsage()
        {
            var casheKey = $"Usage:{currentService.TenantId}";

            try
            {
                var cashedUsage = await cacheService.GetAsync<UsageResponse>(casheKey);
                if (cashedUsage != null)
                    return cashedUsage;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to read usage from cache for tenant {TenantId}, falling back to database.", currentService.TenantId);
            }

            UsageResponse usageprs;
            try
            {
                var usage = await context.usages.FirstOrDefaultAsync(x => x.TenantsId == currentService.TenantId);
                if (usage == null)
                    return new UsageResponse();

                usageprs = new UsageResponse
                {
                    RolesCount = usage.RolesCount,
                    UsersCount = usage.UsersCount,
                    StorageUsedGb = usage.StorageUsedGb,
                    ApiCallsCount = usage.ApiCallsCount,
                    NotificatiomCount = usage.NotificatiomCount
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch usage from database for tenant {TenantId}.", currentService.TenantId);
                throw; 
            }

            try
            {
                await cacheService.SetAsync(casheKey, usageprs, TimeSpan.FromMinutes(10));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to write usage to cache for tenant {TenantId}, continuing without caching.", currentService.TenantId);
            }

            return usageprs;
        }
        public async Task UpdateRolesCount(Guid tenantId)
        {
            var count = await context.roles.CountAsync(x => x.TenantId == tenantId); 
            var usage = await context.usages.FirstOrDefaultAsync(x => x.TenantsId == tenantId);
            if (usage == null)
            {
                usage = new Domain.Entities.UsageRecord { TenantsId = tenantId };
                context.usages.Add(usage);
            }
            usage.RolesCount = count;
            usage.LastUpdated = DateTime.UtcNow;
            await context.SaveChangesAsync();

            try
            {
                await cacheService.RemoveAsync($"Usage:{tenantId}");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Cache removal failed, continuing without cache invalidation.");
            }
        }

        public async Task UpdateUsersCount(Guid tenantId)
        {
            var count = await context.users.CountAsync(x => x.TenantId == tenantId); 
            var usage = await context.usages.FirstOrDefaultAsync(x => x.TenantsId == tenantId);
            if (usage == null)
            {
                usage = new Domain.Entities.UsageRecord { TenantsId = tenantId };
                context.usages.Add(usage);
            }
            usage.UsersCount = count;
            usage.LastUpdated = DateTime.UtcNow;
            await context.SaveChangesAsync();

            try
            {
                await cacheService.RemoveAsync($"Usage:{tenantId}");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Cache removal failed, continuing without cache invalidation.");
            }
        }
        public async Task IncrementUsersCount(Guid tenantId, int delta = 1)
        {
            var usage = await context.usages.FirstOrDefaultAsync(x => x.TenantsId == tenantId);
            if (usage == null)
            {
                var currentCount = await context.users.CountAsync(x => x.TenantId == tenantId);
                usage = new Domain.Entities.UsageRecord
                {
                    TenantsId = tenantId,
                    UsersCount = currentCount
                };
                context.usages.Add(usage);
            }
            else
            {
                usage.UsersCount = Math.Max(0, usage.UsersCount + delta); // not accept minus
            }

            usage.LastUpdated = DateTime.UtcNow;
            await context.SaveChangesAsync();

            try
            {
                await cacheService.RemoveAsync($"Usage:{tenantId}");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Cache removal failed, continuing without cache invalidation.");
            }
        }
        public async Task ReconcileUsage(Guid tenantId)
        {
            try
            {
                var usersCount = await context.users.CountAsync(x => x.TenantId == tenantId);
                var rolesCount = await context.roles.CountAsync(x => x.TenantId == tenantId);
                var notificationCount = await context.notification.CountAsync(x => x.tenantId == tenantId);

                var usage = await context.usages.FirstOrDefaultAsync(x => x.TenantsId == tenantId);
                if (usage == null)
                {
                    usage = new Domain.Entities.UsageRecord { TenantsId = tenantId };
                    context.usages.Add(usage);
                }

                usage.UsersCount = usersCount;
                usage.RolesCount = rolesCount;
                usage.NotificatiomCount = notificationCount;
                usage.LastUpdated = DateTime.UtcNow;

                await context.SaveChangesAsync();

                try
                {
                    await cacheService.RemoveAsync($"Usage:{tenantId}");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Cache removal failed during reconciliation for tenant {TenantId}.", tenantId);
                }

                logger.LogInformation("Usage reconciled for tenant {TenantId}: Users={Users}, Roles={Roles}, Notifications={Notifications}",
                    tenantId, usersCount, rolesCount, notificationCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to reconcile usage for tenant {TenantId}.", tenantId);
                throw;
            }
        }

        public async Task ReconcileAllUsage()
        {
            var tenantIds = await context.subscriptions
                .Select(x => x.TenantId)
                .Distinct()
                .ToListAsync();

            foreach (var tenantId in tenantIds)
            {
                await ReconcileUsage(tenantId);
            }
        }
    }
}
