using BCrypt.Net;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Persistence.Seeding
{
    public static class SuperAdminSeeder
    {
        public static async Task SeddAsync(IApplicationDBContext context)
        {
            var dbContext = (DbContext)context;

            var hasPermissions = await dbContext.Set<Permission>()
                .IgnoreQueryFilters()
                .AnyAsync();

            if (!hasPermissions)
            {
                // seed permissions...
                var existingSuperAdmin = await context.users.AnyAsync(x => x.IsSuperAdmin);
                if (existingSuperAdmin) return;
                var user = new User
                {
                    Email = "mahmoud.kandel.01029841714@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123"),
                    IsSuperAdmin = true
                };
                context.users.Add(user);
                await context.SaveChangesAsync();
            }
           

        }
    }
}
