using CompanyHub.Application.ApiKey.DTOs;
using CompanyHub.Application.Common;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.ApiKey
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currenttenantService;
        public ApiKeyService(IApplicationDBContext context, ICurrenttenantService currenttenantService)
        {
            this.context = context;
            this.currenttenantService = currenttenantService;
        }
        public async Task<Result<CreateApiKeyresponse>> CreateAsync(CreateApiKeyRequest request)
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            var planKey = "chk_" + Convert.ToBase64String(randomBytes)
            .Replace("+", "").Replace("/", "").Replace("=", "");
            var keyhashed = BCrypt.Net.BCrypt.HashPassword(planKey);
            var prefix = planKey.Substring(0, Math.Min(12, planKey.Length));
            var apiKey = new CompanyHub.Domain.Entities.ApiKey
            {
                Id = Guid.NewGuid(),
                tenantId = currenttenantService.TenantId,
                Prefix = prefix,
                KeyHash = keyhashed,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = request.ExpiresAt,
                Name = request.Name,
            };
            context.apiKeys.Add(apiKey);
            await context.SaveChangesAsync();
            return Result<CreateApiKeyresponse>.Success(new CreateApiKeyresponse
            {
                Id = apiKey.Id,
                Name = request.Name,
                PlainKey = planKey,
                ExpiresAt = apiKey.ExpiresAt
            });
        }

        public async Task<Result<List<ApiKeyResponse>>> GetAllAsync()
        {
            var apikeys = await context.apiKeys
                .Where(x=>x.IsActive && (x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(x=>x.CreatedAt)
                .Select(x=>new ApiKeyResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Prefix = x.Prefix,
                    IsActive = x.IsActive,
                    LastUsedAt = x.LastUsedAt,
                    ExpiresAt = x.ExpiresAt,
                    CreatedAt = x.CreatedAt
                }).ToListAsync();
            return Result<List<ApiKeyResponse>>.Success(apikeys);
        }

        public async Task<Result> RevokeAsync(Guid id)
        {
            var apikey = await context.apiKeys.FirstOrDefaultAsync(x => x.Id == id);
            if (apikey == null)
                return Result.Failure("ApiKey not found");
            apikey.IsActive = false;
            await context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Tenants?> ValidateApiKeyAsync(string plainKey)
        {
            if(string.IsNullOrEmpty(plainKey) || !plainKey.StartsWith("chk_"))
                return null;
            var prefix = plainKey.Substring(0, Math.Min(12, plainKey.Length));
            var candidates = await context.apiKeys
                .IgnoreQueryFilters()
                .Where(x => x.Prefix == prefix && x.IsActive && (x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow))
                .Include(x => x.Tenants)
                .ToListAsync();
            foreach (var candidate in candidates)
            {
                if (candidate.ExpiresAt.HasValue && candidate.ExpiresAt.Value < DateTime.UtcNow)
                    continue;

                if (BCrypt.Net.BCrypt.Verify(plainKey, candidate.KeyHash))
                {
                    candidate.LastUsedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync(CancellationToken.None);
                    return candidate.Tenants;
                }
            }

            return null;

        }
    }
}
