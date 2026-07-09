using CompanyHub.Application.Notification.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Notification
{
    public interface INotificationService
    {
        Task Create(string title, string message, Guid? userId = null);
        Task<List<NotificationResponse>> GetAll();
        Task MarkAsRead(Guid Id);
        Task<List<NotificationResponse>> Search(SearchNotificationRequest request);
    }
}
