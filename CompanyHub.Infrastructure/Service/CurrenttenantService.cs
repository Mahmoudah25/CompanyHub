using CompanyHub.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Service
{
    public class CurrenttenantService : ICurrenttenantService
    {
        private readonly IHttpContextAccessor http ;
        public CurrenttenantService(IHttpContextAccessor http)
        {
            this.http = http;
        }
        public Guid UserId =>
            Guid.Parse(
                http.HttpContext?.User.
                FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        public Guid TenantId =>
            Guid.Parse(
                http.HttpContext?.User.FindFirst("TenantId")?.Value ?? Guid.Empty.ToString());

    }
}
