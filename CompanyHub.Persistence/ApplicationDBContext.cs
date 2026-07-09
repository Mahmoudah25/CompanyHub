using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Domain.Entities;
using CompanyHub.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Persistence
{
    public class ApplicationDBContext : DbContext,IApplicationDBContext
    {
        private readonly ICurrenttenantService Service;
        public ApplicationDBContext( DbContextOptions<ApplicationDBContext> options , ICurrenttenantService service) : base (options)  
        {
            // curent teanet Service
            this.Service = service;

        }
        public Guid CurrentTenantId => Service.TenantId;
        // dbsets 
        public DbSet<Tenants> tenants { get; set; } 
        public DbSet<User> users { get; set; }
        public DbSet<UsageRecord> usages { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<RolePermission> rolePermissions { get; set; }  
        public DbSet<Notification> notification { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<Permission> permissions { get; set; }
        public DbSet<Plan> plans { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<Subscription> subscriptions { get; set; }
        public DbSet<UsageRecord> usageRecords { get; set; }
        public DbSet<AuditLog> auditLogs { get; set; }
        public DbSet<UserRole> userRoles { get; set ; }
        public DbSet<PasswordRefreshToken> passwordRefreshToken { get; set; }
        public DbSet<Invoice> invoices { get; set; }
        public DbSet<ApiKey> apiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ==========================
            // Tenant -> Users
            // ==========================
            modelBuilder.Entity<User>()
                .HasOne(u => u.Tenants)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // Tenant -> Roles
            // ==========================
            modelBuilder.Entity<Role>()
                .HasOne(r => r.Tenants)
                .WithMany(t => t.Roles)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // Tenant -> Subscriptions
            // ==========================
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Tenants)
                .WithMany(t => t.Subscription)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // Tenant -> Notifications
            // ==========================
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Tenants)
                .WithMany(t => t.Notification)
                .HasForeignKey(n => n.tenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // Tenant -> UsageRecords
            // ==========================
            modelBuilder.Entity<UsageRecord>()
                .HasOne(u => u.Tenants)
                .WithMany(t => t.UsageRecords)
                .HasForeignKey(u => u.TenantsId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // Tenant -> AuditLogs
            // ==========================
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.tenant)
                .WithMany(t => t.AuditLogs)
                .HasForeignKey(a => a.tenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // User -> RefreshTokens
            // ==========================
            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.users)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================
            // User -> Notifications
            // ==========================
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ==========================
            // User -> AuditLogs
            // ==========================
            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.user)
                .WithMany(u => u.Logs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ==========================
            // Plan -> Subscriptions
            // ==========================
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Plan)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // ==========================
            // Subscription -> Payments
            // ==========================
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.SubSubscription)
                .WithMany(s => s.payments)
                .HasForeignKey(p => p.SubStractionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================
            // UserRole (M:M)
            // ==========================
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.user)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // ==========================
            // RolePermission (M:M)
            // ==========================
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.roleId, rp.PermissinId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.roleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissinId);

            // ==========================
            // User -> RefrehPasswordToken
            // ==========================

            modelBuilder.Entity<PasswordRefreshToken>()
                .HasOne(p => p.User)
                .WithMany(u => u.PasswordRefreshTokens)
                .HasForeignKey(p => p.UserId);

            // ==========================
            // Payment -> Invoice(1,1)
            // ==========================

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Payment)
                .WithOne()
                .HasForeignKey<Invoice>(i => i.PaymentId);

            // ==========================
            // Tenant -> API key(1,M)
            // ==========================
            modelBuilder.Entity<ApiKey>()
                .HasOne(a => a.Tenants)
                .WithMany(t => t.ApiKey)
                .HasForeignKey(a => a.tenantId);
            // plan
            modelBuilder.Entity<Plan>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);
            // Payment
            modelBuilder.Entity<Payment>()
              .Property(x => x.Amount)
               .HasPrecision(18, 2);

            // curent teanet Service(filter)

            modelBuilder.Entity<Role>().
                HasQueryFilter(c => c.TenantId == CurrentTenantId);

            modelBuilder.Entity<User>()
               .HasQueryFilter(u => u.TenantId == CurrentTenantId );

            modelBuilder.Entity<Role>()
                .HasQueryFilter(r => r.TenantId == CurrentTenantId);

            modelBuilder.Entity<Subscription>()
                .HasQueryFilter(s => s.TenantId == CurrentTenantId);

            modelBuilder.Entity<Notification>()
                .HasQueryFilter(n => n.tenantId == CurrentTenantId);

            modelBuilder.Entity<AuditLog>()
                .HasQueryFilter(a => a.tenantId == CurrentTenantId);

            modelBuilder.Entity<UsageRecord>()
                .HasQueryFilter(u => u.TenantsId == CurrentTenantId);

            //
            modelBuilder.Entity<User>()
                 .HasIndex(u => u.Email)
                  .IsUnique();

        }
    }
}
