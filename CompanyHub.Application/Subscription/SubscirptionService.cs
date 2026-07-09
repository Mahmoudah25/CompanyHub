using CompanyHub.Application.Aduit;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Notification;
using CompanyHub.Application.Subscription.DTOs;
using CompanyHub.Application.User.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Subscription
{
    public class SubscirptionService : ISubscirptionService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currnetService;
        private readonly IAduitService aduitService;
        private readonly INotificationService notificationService;
        private readonly INotificationSender notificationSender;
        public SubscirptionService(
            IApplicationDBContext context, ICurrenttenantService currnetService, 
            IAduitService aduitService, INotificationService notificationService, INotificationSender notificationSender)
        {
            this.context = context;
            this.currnetService = currnetService;
            this.aduitService = aduitService;
            this.notificationService = notificationService;
            this.notificationSender = notificationSender;
        }

        public async Task Cancel()
        {
            var sub = await context.subscriptions
                .FirstOrDefaultAsync(x => x.TenantId == currnetService.TenantId && x.IsActive);
            if (sub == null)
                throw new Exception("You don't have an active subscription");
            sub.IsActive = false;
            await context.SaveChangesAsync();
            await aduitService.Log("Cancel Subscription", $"Subscription canceled for tenant '{currnetService.TenantId}'.");
            //await notificationService.Create("Subscription Canceled", "Your subscription has been canceled.");
            await notificationSender.SendAsync(currnetService.UserId, "Subscription Canceled", "Your subscription has been canceled.");
        }

        public async Task Changeplan(ChangePlanRequest request)
        {
            var subscription = await context.subscriptions
                .FirstOrDefaultAsync(x=>x.TenantId == currnetService.TenantId&&x.IsActive);
            if (subscription == null) 
                throw new Exception(" No Active Subscrition");
            var plan = await
                context.plans.FirstOrDefaultAsync(x => x.Id == request.PlanId);
            if (plan == null)
                throw new Exception(" Plan Not Found");
            subscription.PlanId =plan.Id;
            await context.SaveChangesAsync();
            await aduitService.Log("Change Plan", $"Subscription plan changed to '{plan.Name}' for tenant '{currnetService.TenantId}'.");
            await notificationSender.SendAsync(currnetService.UserId, "Subscription Plan Changed", $"Your subscription plan has been changed to '{plan.Name}'.");

        }

        public async Task<SubscriptionResponse?> GetCurrent()
        {
            return await context.subscriptions
                                .Where(x => x.TenantId == currnetService.TenantId && x.IsActive)
                .Select(x => new SubscriptionResponse
                {
                    Id = x.Id,
                    PlanName = x.Plan.Name,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive
                }).FirstOrDefaultAsync();
        }

        public async Task<Guid> Subscribe(CreateSubscriptionRequest request)
        {
            var IsActive = await context.subscriptions.AnyAsync(
                x => x.TenantId == currnetService.TenantId && x.IsActive);
            if(IsActive)
                throw new Exception("You already have an active subscription");
            var plan = await context.plans.FirstOrDefaultAsync(x => x.Id == request.PlanId);
            if (plan == null)
                throw new Exception("Plan not found");
            var NewSubcripton = new CompanyHub.Domain.Entities.Subscription
            {
                TenantId = currnetService.TenantId,
                PlanId = request.PlanId,
                IsActive = true,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };
            context.subscriptions.Add(NewSubcripton);
            await context.SaveChangesAsync();
            await aduitService.Log("Subscribe",
                $"New subscription created for plan '{plan.Name}'.");
            //await notificationService.Create("Subscription Created",
            //    $"Your subscription to plan '{plan.Name}' has been created. Please complete payment.");
            await notificationSender.SendAsync(currnetService.UserId, "Subscription Created",
                $"Your subscription to plan '{plan.Name}' has been created. Please complete payment.");

            return NewSubcripton.Id;
        }

        public Task<List<SubscriptionResponse>> Search(SearchSubscriptionRequest request)
        {
            var query = context.subscriptions.AsQueryable();
            if (request.IsActive.HasValue)
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            if (request.PlanId.HasValue)
                query = query.Where(x => x.PlanId == request.PlanId.Value);
            if(request.From.HasValue)
                query = query.Where(x => x.StartDate >= request.From.Value);
            if (request.EndTo.HasValue)
                query = query.Where(x => x.EndDate <= request.EndTo.Value);
            if(!string.IsNullOrEmpty(request.PlanName))
                query = query.Where(x => x.Plan.Name.Contains(request.PlanName));
            return query.Select(x => new SubscriptionResponse
            {
                Id = x.Id,
                PlanName = x.Plan.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                IsActive = x.IsActive
            }).ToListAsync();
        }
    }
}
