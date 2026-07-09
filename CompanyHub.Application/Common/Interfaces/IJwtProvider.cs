using CompanyHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Common.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(CompanyHub.Domain.Entities.User user, List<string> roles , List<string> permissions);
        ClaimsPrincipal GetPrincpleFromExpiredToken(string token);
        Guid? ValidateTempTokenAndGetUserId(string tempToken);
        string GenerateTempToken(CompanyHub.Domain.Entities.User user);
    }
}
