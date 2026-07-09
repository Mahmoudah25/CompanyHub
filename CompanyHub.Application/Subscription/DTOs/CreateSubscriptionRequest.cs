using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Subscription.DTOs
{
    public class CreateSubscriptionRequest
    {
        public Guid PlanId { get; set; }
    }
}
