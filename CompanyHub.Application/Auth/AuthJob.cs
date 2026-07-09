using CompanyHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Auth
{
    public class AuthJob : IAuthJob
    {
        private readonly IApplicationDBContext context;
        public AuthJob(IApplicationDBContext context)
        {
            this.context = context;
        }

        public async Task EmailVerficationCleanJob()
        {
            var expiredVerification = await
                context.users
                .Where(x => x.EmailVerificationToken != null && !x.EmailVerified || x.EmailVerificationTokenExpiry < DateTime.UtcNow)
                .ToListAsync();
            foreach (var item in expiredVerification)
            {
                item.EmailVerificationToken = null;
                item.EmailVerificationTokenExpiry = null;
            }
            await context.SaveChangesAsync();
        }

        public async Task ToeknClaenJob()
        {
            var cutOff = DateTime.UtcNow.AddDays(-7);
            var expiredToken = await
                context.refreshTokens
                .IgnoreQueryFilters()
                .Where(x => x.Expireddate < cutOff || x.IsRevoked)
                .ToListAsync();
            context.refreshTokens.RemoveRange(expiredToken);
            await context.SaveChangesAsync();
        }
    }
}
