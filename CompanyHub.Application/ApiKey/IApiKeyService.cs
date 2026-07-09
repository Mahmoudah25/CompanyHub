using CompanyHub.Application.ApiKey.DTOs;
using CompanyHub.Application.Common;
using CompanyHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.ApiKey
{
    public interface IApiKeyService
    {
        Task<Result<CreateApiKeyresponse>> CreateAsync(CreateApiKeyRequest request);
        Task<Result<List<ApiKeyResponse>>> GetAllAsync();
        Task<Result> RevokeAsync(Guid id);
        Task<Tenants?> ValidateApiKeyAsync(string plainKey);
    }
}
