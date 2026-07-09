using CompanyHub.Application.Aduit;
using CompanyHub.Application.Common.Exceptions;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Notification;
using CompanyHub.Application.Plan;
using CompanyHub.Application.UsageRecord;
using CompanyHub.Application.User.DTOs;
using CompanyHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuestPDF.Helpers.Colors;

namespace CompanyHub.Application.User
{
    public class UserService : IUserService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService Service;
        private readonly IUsageService usageService;
        private readonly IAduitService aduitService;
        private readonly INotificationService notificationService;
        private readonly INotificationSender notificationSender;
        private readonly IPlanLimitService planLimitService;
        private readonly IEmailService emailService;


        public UserService
            (IApplicationDBContext context, ICurrenttenantService Service,
            IUsageService usageService, IAduitService aduitService, INotificationService notificationService, 
            INotificationSender notificationSender,IPlanLimitService planLimitService,IEmailService emailService)
        {
            this.context = context;
            this.Service = Service;
            this.usageService = usageService;
            this.aduitService = aduitService;
            this.notificationService = notificationService;
            this.notificationSender = notificationSender;
            this.planLimitService = planLimitService;
            this.emailService = emailService;
        }

        public async Task AssignRoleToUser(Guid UserId, AssignRoleToUserRequest request)
        {
            var user = await context.users
                .FirstOrDefaultAsync(c => c.Id == UserId );
            if (user is null) throw new Exception();

            var requestRolwIds = request.RoleIds.Distinct().ToList();
            var validRoleIds = await context.roles.
                Where(r=>requestRolwIds.Contains(r.Id))
                .Select(r=>r.Id)
                .ToListAsync();    
            var invalidRoleIds = requestRolwIds.Except(validRoleIds).ToList();  
            if(invalidRoleIds.Any()) 
                throw new KeyNotFoundException($"The following RoleIds do not exist for this tenant: {string.Join(", ", invalidRoleIds)}");

            var exitingRoles = context.userRoles.Where(x => x.UserId == UserId);
            context.userRoles.RemoveRange(exitingRoles);
            var userRoles = request.RoleIds
                .Select(roleId => new UserRole
                {
                    UserId = UserId,
                    RoleId = roleId
                });
            await context.userRoles.AddRangeAsync(userRoles);
            await context.SaveChangesAsync();
            await aduitService.Log("Assign Role", $" Role assigned to user {user.Email}");
        }
        public async Task RemoveRoleFromUser(Guid UserId, Guid RoleId)
        {
            var userRole = await
                context.userRoles
                .FirstOrDefaultAsync(x => x.RoleId == RoleId && x.UserId == UserId);
            if(userRole == null)
                throw new Exception(" User Role Not found");
            context.userRoles.Remove(userRole);
            await context.SaveChangesAsync();

        }


        public async Task<Guid> CreateUser(CreateUserRequest request)
        {
            var limitCheck = await planLimitService.CheckUserLimitAsync(Service.TenantId);
            if (!limitCheck.IsSuccess)
                throw new InvalidOperationException(limitCheck.Error ?? "User limit exceeded."); // ✅

            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                TenantId = Service.TenantId,
            };

            context.users.Add(user);
            await context.SaveChangesAsync();
            await usageService.IncrementUsersCount(Service.TenantId, +1);
            await aduitService.Log("User Created", $"User {user.Email} Created");
            await emailService.SendEmail(
                         user.Email,
                       "Welcome to CompanyHub",
                       $"<h1>Welcome, {user.FirstName}!</h1>" +
                       $"<p>Your account has been created successfully by your organization admin.</p>" +
                       $"<p>You can now log in using your email and the password provided to you.</p>");
              return user.Id;
        }
        public async Task<bool> Delete(Guid Id)
        {
            var found = await context.users.FirstOrDefaultAsync(c => c.Id == Id);
            if (found is null) return false;

            context.users.Remove(found);
            await context.SaveChangesAsync();
            await usageService.IncrementUsersCount(Service.TenantId, -1); 

            await aduitService.Log("User Deleted", $"User {found.Email} Deleted");
            return true;
        }

        public async Task<List<UserResponse>> GetAllUsers()
        {
            var users = await context.users
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                })
                .ToListAsync();
            return users;
        }

        public async Task<UserResponse> getUserByEmail(string email)
        {
            var user = await context.users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                throw new KeyNotFoundException($"User with email {email} not found.");
            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public async Task<UserResponse> GetUsersById(Guid Id)
        {
            var user = await context.users.FirstOrDefaultAsync(o=>o.Id == Id);
            if (user is null) throw new KeyNotFoundException();
            return new UserResponse
            {
                Id= user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public async Task<bool> UpdateUser(Guid Id, UpdateUserRequest request)
        {
            var found = await context.users.FirstOrDefaultAsync(x=>x.Id == Id);
            if(found is null) return false;
            found.FirstName = request.FirstName;
            found.LastName = request.LastName;
            found.Email = request.Email;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserResponse>> SearchUser(SearchUserRequest request)
        {
            var query = context.users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(x =>
                    (x.FirstName + " " + x.LastName).Contains(request.Name));
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                query = query.Where(x =>
                    x.Email.Contains(request.Email));
            }

            return await query
                .Select(x => new UserResponse
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email
                })
                .ToListAsync();
        }
    }
}
