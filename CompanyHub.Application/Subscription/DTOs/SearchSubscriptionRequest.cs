using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Subscription.DTOs
{
    public class SearchSubscriptionRequest
    {
        public bool? IsActive { get; set; }  
        public Guid? PlanId { get; set; }
        public string? PlanName { get; set; }= string.Empty;
        public DateTime? From { get; set; }
        public DateTime? EndTo { get; set; }
    }
}
