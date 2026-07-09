using CompanyHub.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace CompanyHub.API.Middleware
{
    public class GlobalExpectionHandler(RequestDelegate next, ILogger<GlobalExpectionHandler> logger)
    {
        private readonly RequestDelegate next = next;
        private readonly ILogger<GlobalExpectionHandler> logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Resource not found");
                await HandleExpectiomAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogWarning(ex, "Unauthorized access attempt");
                await HandleExpectiomAsync(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (BusinessRuleException ex)
            {
                logger.LogWarning(ex, "Business rule violation");
                await HandleExpectiomAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid operation");
                await HandleExpectiomAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
                await HandleExpectiomAsync(context, HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later.");
            }
        }

        private static async Task HandleExpectiomAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var response = new ErrorResponse
            {
                Status = (int)statusCode,
                Message = message
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}