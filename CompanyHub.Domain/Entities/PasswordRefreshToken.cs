using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class PasswordRefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
