using CompanyHub.Application.Notification;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Hubs
{
    public class SignalRNotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> hubContext;
        public SignalRNotificationSender(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task SendAsync(Guid userId, string title, string message)
        {
            await hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", 
                new {
                    title, 
                    message 
                });
        }
    }
}
