using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Notification;
using CompanyHub.Application.Plan.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Plan
{
    public class PlanService : IPlanService
    {
        private readonly IApplicationDBContext context;
        private readonly INotificationService notificationService;
        private readonly ICacheService cacheService;
        public PlanService(IApplicationDBContext context, INotificationService notificationService, ICacheService cacheService)
        {
            this.context = context;
            this.notificationService = notificationService;
            this.cacheService = cacheService;
        }

        public async Task<PlanResponse> CreatePlan(CreateplanRequest request)
        {
            var plan = new CompanyHub.Domain.Entities.Plan
            {
                Name = request.Name,
                Price = request.Price,
                MaxRoles = request.MaxRoles,
                MaxUsers = request.MaxUsers,
                StrorageLimitMB = request.StrorageLimitMB,
            };
            context.plans.Add(plan);
            await context.SaveChangesAsync();
            await cacheService.RemoveAsync("plans");
            await notificationService.Create($" Plan Updated",$" Plan Changed To {plan.Name}");
            return new PlanResponse
            {
                Name =plan.Name,
                Price = plan.Price,
                MaxRoles = plan.MaxRoles,
                MaxUsers = plan.MaxUsers,
                StrorageLimitMB = plan.StrorageLimitMB,
            };
        }

        public async Task<bool> DeletePlan(Guid planId)
        {
            var foundrPlan = await context.plans.FirstOrDefaultAsync(p => p.Id == planId);
            if (foundrPlan != null) 
            {
                context.plans.Remove(foundrPlan);
                await context.SaveChangesAsync();
                await cacheService.RemoveAsync("plans");
                return true;
            }
            return false;
        }

        public async Task<List<Domain.Entities.Plan>> GetAllPlans()
        {
            //const string cacheKey = "plans";
            //var cahedpalns = await cacheService.GetAsync<List<Domain.Entities.Plan>>(cacheKey);
            //if(cahedpalns != null)
            //    return cahedpalns;
            //var plans = await context.plans.ToListAsync();
            //await cacheService.SetAsync(cacheKey, plans,TimeSpan.FromMinutes(30));
            //return plans;

            const string cacheKey = "plans";

            try
            {
                var cachedPlans = await cacheService.GetAsync<List<Domain.Entities.Plan>>(cacheKey);
                if (cachedPlans != null)
                    return cachedPlans;
            }
            catch
            {
                // لو Redis مش شغال، نكمل من الداتابيز
                return await context.plans.ToListAsync();
            }

            var plans = await context.plans.ToListAsync();

            try
            {
                await cacheService.SetAsync(cacheKey, plans, TimeSpan.FromMinutes(30));
            }
            catch
            {
                // لو Redis مش شغال، نرجع البيانات من غير Cache
            }

            return plans;
            //return await context.plans.ToListAsync();

        }

        public async Task<PlanResponse> GetPlanById(Guid planId)
        {
            var role = await context.plans.FirstOrDefaultAsync(x=>x.Id == planId);
            if (role == null) return null;
            return new PlanResponse
            {
                Name=role.Name,
                Price = role.Price,
                MaxRoles = role.MaxRoles,
                MaxUsers = role.MaxUsers,
                StrorageLimitMB = role.StrorageLimitMB,
            };
        }

        public async Task<PlanResponse> GetPlanByName(string planName)
        {
            var role = await context.plans.FirstOrDefaultAsync(x => x.Name == planName);
            if (role == null) return null;
            return new PlanResponse
            {
                Name = role.Name,
                Price = role.Price,
                MaxRoles = role.MaxRoles,
                MaxUsers = role.MaxUsers,
                StrorageLimitMB = role.StrorageLimitMB,
            };      
        }

        public async Task<List<PlanResponse>> SearchPlanRequests(SearchPlanRequest request)
        {
            var query = context.plans.AsQueryable();
            if(!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(x=>x.Name.Contains(request.Name));
            if (request.MinPrice.HasValue) 
                query = query.Where(x=>x.Price >= request.MinPrice.Value);
            if(request.MaxPrice.HasValue)
                query = query.Where(x=>x.Price <= request.MaxPrice.Value);
            return await query.Select
                (x => new PlanResponse
                {
                    Name = x.Name,
                    Price = x.Price
                }).ToListAsync();
            
        }

        public async Task<bool> UpdatePlan(Guid planId, UpdatePlanRequest request)
        {
           var foundPlan = await context.plans.FirstOrDefaultAsync(x => x.Id == planId);
            if (foundPlan != null)
            {
                foundPlan.Name = request.Name;
                foundPlan.Price = request.Price;
                foundPlan.MaxRoles = request.MaxRoles;
                foundPlan.MaxUsers = request.MaxUsers;
                foundPlan.StrorageLimitMB = request.StrorageLimitMB;
                await context.SaveChangesAsync();
                await cacheService.RemoveAsync("plans");
                return true;
            }
            return false;
        }
    }
}
