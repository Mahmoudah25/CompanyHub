using CompanyHub.Application.Aduit;
using CompanyHub.Application.Common.Exceptions;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Notification;
using CompanyHub.Application.Plan;
using CompanyHub.Application.Role.DTOs;
using CompanyHub.Application.UsageRecord;
using CompanyHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Role
{
    public class RoleService : IRoleService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService service;
        private readonly IUsageService usageService;
        private readonly IAduitService aduitService;
        private readonly INotificationService notificationService;
        private readonly INotificationSender notificationSender;
        private readonly IPlanLimitService planLimitService;

        public RoleService
            (IApplicationDBContext context, ICurrenttenantService service,
            IUsageService usageService, IAduitService aduitService,INotificationService notificationService
            , INotificationSender notificationSender,IPlanLimitService planLimitService)
        {
            this.context = context;
            this.service = service;
            this.usageService = usageService;
            this.aduitService = aduitService;
            this.notificationService = notificationService;
            this.notificationSender = notificationSender;
            this.planLimitService = planLimitService;
        }


        public async Task<Guid> CreateRole(CreateRoleRequest request)
        {
            var newrole = new CompanyHub.Domain.Entities.Role
            {
                Name = request.Name,
                TenantId = service.TenantId,
            };

            var subscription = await context.subscriptions
                .FirstOrDefaultAsync(x => x.TenantId == service.TenantId);
            if (subscription == null)
                throw new InvalidOperationException("No active subscription found for the tenant."); // ✅

            var limitCheck = await planLimitService.CheckRoleLimitAsyunc(service.TenantId);
            if (!limitCheck.IsSuccess)
                throw new InvalidOperationException(limitCheck.Error ?? "Role limit exceeded."); // ✅ ورسالة حقيقية

            context.roles.Add(newrole);
            await usageService.UpdateRolesCount(service.TenantId);
            await context.SaveChangesAsync();
            await aduitService.Log("Create Role", $"Role '{newrole.Name}' created.");
            await notificationSender.SendAsync(service.UserId, "Role Created", $"Role '{newrole.Name}' has been created.");
            return newrole.Id;
        }

        public async Task<bool> DeleteRoleAsync(Guid Id)
        {
            var role = context.roles.FirstOrDefault(x => x.Id == Id) ;
            if (role == null) return false;
            context.roles.Remove(role);
            await usageService.UpdateRolesCount(service.TenantId);
            await context.SaveChangesAsync();
            await aduitService.Log("Delete Role", $"Role '{role.Name}' deleted.");
            return true;
        }

        public async Task<List<RoleResponse>> GetAllRolesAsync()
        {
            var roles = await context.roles
                .Select(x=> new RoleResponse
                {
                    Id=x.Id,
                    Name=x.Name
                })
                .ToListAsync();
            return roles;
        }

        public async Task<RoleResponse> GetRolesByIdAsync(Guid id)
        {
            var role = await
                context.roles
                .FirstOrDefaultAsync(x => x.Id == id);
            if (role is null) return null;
            var roleId = new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
            };
            return roleId;

        }

        public async Task<RoleResponse> GetRolesByNameAsync(string Name)
        {
            var role = await context.roles
                .FirstOrDefaultAsync(x=>x.Name == Name);
            var res = new RoleResponse
            {
                Id = role.Id,
                Name = Name,
            };
            return res;
        }

        public async Task<bool> UpdateRoleAsync(Guid Id, UpdateRoleRequest request)
        {
            var FindRole = await context.roles
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (FindRole == null) return false;
            FindRole.Name = request.Name;   
            await context.SaveChangesAsync();
            return true;
        }

        //AssignPermissionsToRole
        public async Task AssignPermissionsToRole(Guid RoleId, AssignPermissionsRequest request)
        {
            var role = await context.roles
                .FirstOrDefaultAsync(context => context.Id == RoleId);
            if (role == null)
                throw new Exception();

            var requestPermissionIds = request.PermissionsIds.Distinct().ToList();  
            var ValidPermissionIds = await context.permissions
                .Where(p=>requestPermissionIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();
            var InValidPermissionIds = requestPermissionIds.Except(ValidPermissionIds).ToList();
            if(InValidPermissionIds.Any())
                throw new Exception($"The following PermissionIds do not exist: {string.Join(", ", InValidPermissionIds)}");

            var oldPermission = context.rolePermissions.Where(x => x.roleId == RoleId);
            context.rolePermissions.RemoveRange(oldPermission);
            var rolePermission = request.PermissionsIds.Select(permissonId => new RolePermission
            {
                roleId = RoleId,
                PermissinId = permissonId,
            });
            await context.rolePermissions.AddRangeAsync(rolePermission);
            await context.SaveChangesAsync();
        }
    }
}
