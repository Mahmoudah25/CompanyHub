using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Notification.DTOs
{
    public class SearchNotificationRequest
    {
        public bool? IsRead { get; set; }
        public string? Title { get; set; }
    }
}
