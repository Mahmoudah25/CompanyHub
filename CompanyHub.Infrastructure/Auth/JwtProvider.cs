using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtSetting Setting;
        public JwtProvider(IOptions<JwtSetting> options)
        {
            Setting = options.Value;
        }
        public string GenerateToken(User user , List<string> roles, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("TenantId",user.TenantId.ToString()),
                new Claim("IsSuperAdmin",user.IsSuperAdmin.ToString())
            };
            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
                
            }
            foreach (var item in permissions)
            {
                claims.Add(new Claim("Permission", item));
            }
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Setting.Key));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Setting.Issuer,
                audience: Setting.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Setting.DurationInMinutes),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincpleFromExpiredToken(string token)
        {
            var tokenValifationParameter = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = Setting.Issuer,
                ValidAudience = Setting.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Setting.Key))
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal =
                tokenHandler.ValidateToken
                (token, tokenValifationParameter, out SecurityToken securityToken);
            return principal;
        }


        public string GenerateTempToken(User user)
        {
            var claims = new List<Claim>
    {
         new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
         new Claim("purpose", "2fa_pending")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Setting.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Setting.Issuer,
                audience: Setting.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Guid? ValidateTempTokenAndGetUserId(string tempToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Setting.Issuer,
                    ValidateAudience = true,
                    ValidAudience = Setting.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Setting.Key)),
                    ValidateLifetime = true
                };

                var principal = handler.ValidateToken(tempToken, validationParams, out _);

                var purpose = principal.FindFirst("purpose")?.Value;
                if (purpose != "2fa_pending")
                    return null;

                var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                return Guid.TryParse(sub, out var userId) ? userId : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
