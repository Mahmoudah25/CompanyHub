using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Persistence.Seeding
{
    public class PermissionSeeder
    {
        public static async Task SeedAsync(ApplicationDBContext context)
        {
            
            var permissions = new List<Domain.Entities.Permission>
            {
                // User Permissions
                new Domain.Entities.Permission { Name = "User.Create" },
                new Domain.Entities.Permission { Name = "User.Read" },
                new Domain.Entities.Permission { Name = "User.Update" },
                new Domain.Entities.Permission { Name = "User.Delete" },
                // role Permissions
                new Domain.Entities.Permission { Name = "Role.Create" },
                new Domain.Entities.Permission { Name = "Role.Read" },
                new Domain.Entities.Permission { Name = "Role.Update" },
                new Domain.Entities.Permission { Name = "Role.Delete" },
                new Domain.Entities.Permission { Name = "Role.AssignPermission" },
                // substraction Permissions
                new Domain.Entities.Permission { Name = "Substraction.Read" },
                new Domain.Entities.Permission { Name = "Substraction.Update" },
                new Domain.Entities.Permission { Name = "Substraction.Create" },
                // Payment Permissions
                new Domain.Entities.Permission { Name = "Payment.Create" },
                new Domain.Entities.Permission { Name = "Payment.Read" },
                new Domain.Entities.Permission { Name = "Payment.Update" },
                //Aduit
                new Domain.Entities.Permission { Name = "Aduit.Read" },
                // Dashboard
                new Domain.Entities.Permission { Name = "Dashboard.Read" },
                // Report
                new Domain.Entities.Permission { Name = "Report.Read" },
            };
            foreach(var permission in permissions)
            {
                if(!await context.permissions.AnyAsync(x=>x.Name == permission.Name))
                {
                    context.permissions.Add(permission);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
    