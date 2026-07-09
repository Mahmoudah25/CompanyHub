using CompanyHub.Application.ApiKey;
using System.Security.Claims;

namespace CompanyHub.API.Middleware
{
    public class ApiKeyMiddleware(RequestDelegate next)
    {
        private const string HeaderName = "X-API-KEY";
        public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
        {
            if(!context.Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
            {
                await next(context);
                return;
            }
            var tenant = await apiKeyService.ValidateApiKeyAsync(extractedApiKey.ToString());
            if (tenant is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid or expired API Key" });
                return;
            }
            var claims = new List<Claim>
            {
                  new Claim("TenantId", tenant.Id.ToString()),
                  new Claim("AuthMethod", "ApiKey")
            };
            var identity = new ClaimsIdentity(claims, "ApiKey");
            context.User = new ClaimsPrincipal(identity);

            await next(context);
        }


    }
}
