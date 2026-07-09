using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Subscription.DTOs
{
    public class SubscriptionsOverviewResponsive
    {
        public int ActiveCount { get; set; }
        public int CancelledCount { get; set; }
        public Dictionary<string, int> ByPlan { get; set; } = new(); // "Basic": 5, "Pro": 3
    }
}
