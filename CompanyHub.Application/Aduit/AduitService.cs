using CompanyHub.Application.Aduit.DTOs;
using CompanyHub.Application.Common;
using CompanyHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Aduit
{
    public class AduitService : IAduitService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currentService;
        public AduitService(IApplicationDBContext context, ICurrenttenantService currentService)
        {
            this.context = context;
            this.currentService = currentService;
        }

        public async Task<Result<List<AuditLogResponse>>> GetAllAsync()
        {
            var currentUser = await context.users
                .FirstOrDefaultAsync(x => x.Id == currentService.UserId); 

            if (currentUser is null || !currentUser.IsSuperAdmin)
                return Result<List<AuditLogResponse>>.Failure("Only SuperAdmins can view all logs.");

            var logs = await context.auditLogs
                .OrderByDescending(x => x.CreateAt)
                .Select(x => new AuditLogResponse
                {
                    Id = x.Id,
                    Action = x.Action,
                    Details = x.Details,
                    CreateAt = x.CreateAt
                })
                .ToListAsync();

            return Result<List<AuditLogResponse>>.Success(logs);
        }

        public async Task<List<AuditLogResponse>> GetLogs()
        {
            return await
                context.auditLogs.Where(x=>x.tenantId == currentService.TenantId)
                .OrderByDescending(x => x.CreateAt)
                .Select(x=> new AuditLogResponse
                {
                    Action = x.Action,
                    Details = x.Details,
                    CreateAt = x.CreateAt,
                    Id = x.Id
                }).ToListAsync();
        }

        public async Task Log(string action, string details)
        {
            var log = new Domain.Entities.AuditLog
            {
                Action = action,
                Details = details,
                tenantId = currentService.TenantId,
                UserId = currentService.UserId,
                CreateAt = DateTime.UtcNow
            };
            context.auditLogs.Add(log);
            await context.SaveChangesAsync();
        }

        public async Task<Result<PaginatedList<AuditLogResponse>>> SearchAsync(SearchAuditLogRequest request)
        {
            var query = context.auditLogs
                .Where(x => x.tenantId == currentService.TenantId) // 🔒 ضروري جداً
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Action))
                query = query.Where(x => x.Action.Contains(request.Action));

            if (request.UserId.HasValue)
                query = query.Where(x => x.UserId == request.UserId.Value);

            if (request.FromDate.HasValue)
                query = query.Where(x => x.CreateAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(x => x.CreateAt <= request.ToDate.Value);

            query = query.OrderByDescending(x => x.CreateAt);

            var mapped = query.Select(a => new AuditLogResponse
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.user != null ? a.user.FirstName : null,
                Action = a.Action,
                Details = a.Details,
                CreateAt = a.CreateAt
            });

            var result = await PaginatedList<AuditLogResponse>.CreateAsync(
                mapped, request.PageNumber, request.PageSize);

            return Result<PaginatedList<AuditLogResponse>>.Success(result);
        }
    }
}
