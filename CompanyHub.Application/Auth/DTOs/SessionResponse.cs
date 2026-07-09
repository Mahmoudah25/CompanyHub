using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth.DTOs
{
    public class SessionResponse
    {
            public Guid Id { get; set; }
            public string? DeviceInfo { get; set; }
            public string? IpAddress { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public bool IsCurrentSession { get; set; } // الجلسة اللي بيستخدمها دلوقتي بالظبط
    }
    
}
