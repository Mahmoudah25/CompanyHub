using CompanyHub.Application.Common;
using CompanyHub.Application.Tenant.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Tenant
{
    public interface ITeanantService
    {
        Task<Result<List<TenantResponse>>> GetAllAsync();
        Task<Result<TenantResponse>> GetByIdAsync(Guid id);
        Task<Result<TenantResponse>> UpdateAsync(Guid id, UpdateTenantRequest request);
        Task<Result<PaginatedList<TenantResponse>>> SearchAsync(SearchTenantRequest request);
        Task<Result> DeactivateAsync(Guid id);
        Task<Result> ActivateAsync(Guid id);
        Task<Result> DeleteAsync(Guid id);
        Task<Result> RestoreAsync(Guid id);
        Task<Result<string>> UploadLogoAsync(Guid tenantId, IFormFile file);
    }
}
