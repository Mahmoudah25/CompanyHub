using CompanyHub.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.RefreshToken
{
    public class RefreshTokenJob : IRefreshTokenJob
    {
        private readonly IApplicationDBContext context;
        public RefreshTokenJob(IApplicationDBContext context)
        {
            this.context = context;
        }
        public async Task RemoveExpiredTokens()
        {
            var tokens  = await 
                context.refreshTokens.Where(x => x.Expireddate < DateTime.UtcNow).ToListAsync();
            context.refreshTokens.RemoveRange(tokens);
            await context.SaveChangesAsync();
        }
    }
}
