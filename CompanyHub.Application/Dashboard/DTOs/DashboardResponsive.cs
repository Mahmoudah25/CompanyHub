using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Dashboard.DTOs
{
    public class DashboardResponsive
    {
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalNotifications { get; set; }
        public int Totalpayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public string CurrentPlan { get; set; } = string.Empty;
        public bool SubscriptionActive { get; set; }
    }
}
