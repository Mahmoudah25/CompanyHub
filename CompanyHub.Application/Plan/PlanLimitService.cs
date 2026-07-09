using CompanyHub.Application.Common;
using CompanyHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Plan
{
    public class PlanLimitService : IPlanLimitService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currentUserService;
        public PlanLimitService(IApplicationDBContext context, ICurrenttenantService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }
        public async Task<Result> CheckRoleLimitAsyunc(Guid TeanatId)
        {
            var subscription = context.subscriptions
                .Include(x => x.Plan)
                .FirstOrDefault(s => s.TenantId == TeanatId && s.IsActive);
            if (subscription == null)
                return await Task.FromResult(Result.Failure("No active subscription found for the tenant."));
            var countRole = await context.roles.CountAsync(r => r.TenantId == TeanatId);
            return countRole < subscription.Plan.MaxRoles
                ? await Task.FromResult(Result.Success())
                : await Task.FromResult(Result.Failure("Role limit exceeded for the current plan."));
        }

        public async Task<Result> CheckUserLimitAsync(Guid TeanatId)
        {
            var subscription = context.subscriptions
                .Include(x=>x.Plan)
                .FirstOrDefault(s => s.TenantId == TeanatId && s.IsActive);
            if(subscription == null)
                return await Task.FromResult(Result.Failure("No active subscription found for the tenant."));
            var countUser = await context.users.CountAsync(u => u.TenantId == TeanatId);
            return countUser < subscription.Plan.MaxUsers
                ? await Task.FromResult(Result.Success())
                : await Task.FromResult(Result.Failure("User limit exceeded for the current plan."));

        }
    }
}
