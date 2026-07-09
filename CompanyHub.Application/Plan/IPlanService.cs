using CompanyHub.Application.Plan.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Plan
{
    public interface IPlanService
    {
        Task<PlanResponse> CreatePlan(CreateplanRequest request);
        Task<List<Domain.Entities.Plan>> GetAllPlans();
        Task<PlanResponse> GetPlanById(Guid planId);
        Task<PlanResponse> GetPlanByName(string planName);
        Task<bool> UpdatePlan(Guid planId, UpdatePlanRequest request);
        Task<bool> DeletePlan(Guid planId);
        Task<List<PlanResponse>> SearchPlanRequests(SearchPlanRequest request);
    }
}
