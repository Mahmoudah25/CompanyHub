using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Subscription.DTOs
{
    public class SubscriptionResponse
    {
        public Guid Id { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public decimal price { get; set; }  
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
