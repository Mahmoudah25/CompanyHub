using CompanyHub.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CompanyHub.Infrastructure.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            //  Super Admin don't care Permission check
            var superAdminClaim = context.User.FindFirst(x => x.Type == "IsSuperAdmin");
            if (superAdminClaim?.Value == "True")
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var permissions = context.User
                .FindAll("Permission")
                .Select(x => x.Value);

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}