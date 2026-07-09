using CompanyHub.Application.Aduit.DTOs;
using CompanyHub.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Aduit
{
    public interface IAduitService
    {
        Task Log(string action, string details);
        Task<List<AuditLogResponse>> GetLogs();
        Task<Result<List<AuditLogResponse>>> GetAllAsync();
        Task<Result<PaginatedList<AuditLogResponse>>> SearchAsync(SearchAuditLogRequest request);
    }
}
