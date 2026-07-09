using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string token {  get; set; } = string.Empty;
        public DateTime Expireddate { get; set; }
        public DateTime? RevokeTime { get; set; }
        // Important for session management, if the user logs out or the token is revoked, we can set this to true
        public bool IsRevoked { get; set; } = false;
        public string? IpAddress { get; set; }
        public string? DeviceInfo { get; set; } 

        //FK
        public Guid UserId { get; set; }
        public User users { get; set; } = null!;
    }
}
