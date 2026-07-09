using CompanyHub.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Authorization
{
    public class SuperAdminHandler : AuthorizationHandler<SuperAdminRequirment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdminRequirment requirement)
        {
            var claim  = context.User.FindFirst(x => x.Type == "IsSuperAdmin");
            if(claim!.Value == "True")
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
