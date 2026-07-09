using CompanyHub.Application.UsageRecord.DTOs;
using CompanyHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.UsageRecord
{
    public interface IUsageService
    {
        Task UpdateUsersCount(Guid tenantId);
        Task UpdateRolesCount(Guid tenantId);
        Task<UsageResponse> GetUsage();
        Task IncrementUsersCount(Guid tenantId, int delta = 1);
        Task ReconcileUsage(Guid tenantId);
        Task ReconcileAllUsage(); // For Only Super Admin
    }
}
