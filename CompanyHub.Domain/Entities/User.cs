using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; }  
        // FK
        [ForeignKey("Tenants")]
        public Guid? TenantId {  get; set; }
        public Tenants Tenants { get; set; } = null!; // tenant cant be null
        // relations
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<AuditLog> Logs { get; set; } = new List<AuditLog>();
        public ICollection<PasswordRefreshToken> PasswordRefreshTokens { get; set; } = new List<PasswordRefreshToken>();

        // 2FA
        public bool TwoFactorEnabled {  get; set; } = false;
        public string? TwoFactorSecret {  get; set; }
        // Email Verification
        public bool EmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
    }
}
