using CompanyHub.Application.Notification;
using CompanyHub.Application.Notification.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;
        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
        public async Task <IActionResult> GetNotification()
        {
            var notifications = await notificationService.GetAll();
            return Ok(notifications);
        }

        [HttpPut("{Id}/read")]
        public async Task <IActionResult> MarkAsRead(Guid Id)
        {
            await notificationService.MarkAsRead(Id);
            return Ok();
        }
        [HttpGet("search")]
        public async Task <IActionResult> Search([FromQuery] SearchNotificationRequest request)
        {
            var notifications = await notificationService.Search(request);
            return Ok(notifications);
        }
    }
}
