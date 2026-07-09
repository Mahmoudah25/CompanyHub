using CompanyHub.Application.Subscription.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Subscription
{
    public interface ISubscirptionService
    {
        Task<Guid> Subscribe(CreateSubscriptionRequest request);
        Task<SubscriptionResponse?> GetCurrent();
        Task Changeplan(ChangePlanRequest request);
        Task Cancel();
        Task<List<SubscriptionResponse>> Search(SearchSubscriptionRequest request);
    }
}
