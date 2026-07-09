using CompanyHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Common.Interfaces
{
    public interface IApplicationDBContext
    {
        DbSet<Tenants> tenants { get; set; }
        DbSet<CompanyHub.Domain.Entities.User> users { get; set; }
        DbSet<CompanyHub.Domain.Entities.UsageRecord> usages { get; set; }
        DbSet<CompanyHub.Domain.Entities.Role> roles { get; set; }
        DbSet<RolePermission> rolePermissions { get; set; }
        DbSet<CompanyHub.Domain.Entities.Notification> notification { get; set; }
        DbSet<CompanyHub.Domain.Entities.Payment> payments { get; set; }
        DbSet<Permission> permissions { get; set; }
        DbSet<CompanyHub.Domain.Entities.Plan> plans { get; set; }
        DbSet<RefreshToken> refreshTokens { get; set; }
        DbSet<CompanyHub.Domain.Entities.Subscription> subscriptions { get; set; }
        DbSet<CompanyHub.Domain.Entities.UsageRecord> usageRecords { get; set; }
        DbSet<CompanyHub.Domain.Entities.AuditLog> auditLogs { get; set; }
        DbSet<UserRole> userRoles { get; set; }
        DbSet<CompanyHub.Domain.Entities.PasswordRefreshToken> passwordRefreshToken { get; set; }
        public DbSet<CompanyHub.Domain.Entities.Invoice> invoices { get; set; }
        public DbSet<CompanyHub.Domain.Entities.ApiKey> apiKeys { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
