using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Notification
{
    public interface INotificationSender
    {
        Task SendAsync(Guid userId, string title, string message);
    }
}
