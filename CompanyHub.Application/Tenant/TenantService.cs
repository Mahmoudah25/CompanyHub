using CompanyHub.Application.Common;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Tenant.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Tenant
{
    public class TenantService : ITeanantService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currentUserService;
        private readonly IFileStorageService fileStorageService;
        public TenantService(IApplicationDBContext context, ICurrenttenantService currentUserService, IFileStorageService fileStorageService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
            this.fileStorageService = fileStorageService;
        }

        public async Task<Result> ActivateAsync(Guid id)
        {
            var tenant = await context.tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null)
                return Result.Failure("Tenant not found");
            tenant.IsActive = true;
            await context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeactivateAsync(Guid id)
        {
            var tenant = await context.tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null)
                return Result.Failure("Tenant not found");
            tenant.IsActive = false;
            await context.SaveChangesAsync();
            return Result.Success();
        }

        public Task<Result<List<TenantResponse>>> GetAllAsync()
        {
            var tenants = context.tenants
                .IgnoreQueryFilters()
                .Select(t => new TenantResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    SubDomain = t.SubDomain,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreateAt,
                    UsersCount = context.users.Count(u => u.TenantId == t.Id)
                }).ToList();
            return Task.FromResult(Result<List<TenantResponse>>.Success(tenants));
        }

        public async Task<Result<TenantResponse>> GetByIdAsync(Guid id)
        {
            var tenant = await context.tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null)
                return Result<TenantResponse>.Failure("Tenant not found");
            return Result<TenantResponse>.Success(new TenantResponse
            {
                Id = tenant.Id,
                Name = tenant.Name,
                SubDomain = tenant.SubDomain,
                IsActive = tenant.IsActive,
                CreatedAt = tenant.CreateAt,
                UsersCount = await context.users.CountAsync(u => u.TenantId == tenant.Id)
            });
        }

        public Task<Result<PaginatedList<TenantResponse>>> SearchAsync(SearchTenantRequest request)
        {
            var query = context.tenants
                .IgnoreQueryFilters()
                .AsQueryable();
            if (!string.IsNullOrEmpty(request.Name))
                query = query.Where(t => t.Name.Contains(request.Name));
            if (request.IsActive.HasValue)
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            var mapped = query.Select(t => new TenantResponse
            {
                Id = t.Id,
                Name = t.Name,
                SubDomain = t.SubDomain,
                IsActive = t.IsActive,
                CreatedAt = t.CreateAt,
                UsersCount = t.Users.Count
            });

            return PaginatedList<TenantResponse>.CreateAsync(mapped, request.PageNumber, request.PageSize)
                .ContinueWith(t => Result<PaginatedList<TenantResponse>>.Success(t.Result));
        }

        public async Task<Result<TenantResponse>> UpdateAsync(Guid id, UpdateTenantRequest request)
        {
            var tenatnet = await context.tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tenatnet == null)
                return Result<TenantResponse>.Failure("Tenant not found");
            tenatnet.SubDomain = request.SubDomain;
            tenatnet.UpdateAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return Result<TenantResponse>.Success(new TenantResponse
            {
                Id = tenatnet.Id,
                Name = tenatnet.Name,
                SubDomain = tenatnet.SubDomain,
                IsActive = tenatnet.IsActive,
                CreatedAt = tenatnet.CreateAt,
                UsersCount = await context.users.CountAsync(u => u.TenantId == tenatnet.Id)
            });
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var tenant = await context.tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tenant == null)
                return Result.Failure("Tenant not found");
            tenant.IsDeleted = true;
            tenant.IsActive = false;
            await context.SaveChangesAsync();
            return Result.Success();

        }

        public async Task<Result> RestoreAsync(Guid id)
        {
            var tenant = await context.tenants.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x=>x.Id == id);
            if(tenant == null)
                return Result.Failure($"Unable to restore tenants");
            tenant.IsActive = true;
            tenant.IsDeleted = false;
            await context.SaveChangesAsync();
            return Result.Success();

        }
        public async Task<Result<string>> UploadLogoAsync(Guid tenantId, IFormFile file)
        {
            var tenant = await context.tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant == null)
                return Result<string>.Failure("Tenant not found");
            try
            {
                if (!string.IsNullOrEmpty(tenant.LogoUrl))
                    await fileStorageService.DeleteAsync(tenant.LogoUrl);
                var url = await fileStorageService.UploadAsync(file, "tenant-logos");
                tenant.LogoUrl = url;
                await context.SaveChangesAsync();
                return Result<string>.Success(url);
            }
            catch(ArgumentException ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }

    }
    
}
