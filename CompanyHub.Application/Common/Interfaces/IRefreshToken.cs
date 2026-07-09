using CompanyHub.Application.Auth.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Common.Interfaces
{
    public interface IRefreshToken
    {
        Task<AuthResponse> Refresh(RefreshTokenRequest refreshToken );
    }
}
