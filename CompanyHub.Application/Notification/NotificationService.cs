using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Notification.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currenttenantService;
        private readonly INotificationSender notificationSender;

        public NotificationService(IApplicationDBContext context, ICurrenttenantService currenttenantService,INotificationSender notificationSender)
        {
            this.context = context;
            this.currenttenantService = currenttenantService;
            this.notificationSender = notificationSender;
        }
        public async Task Create(string title, string message , Guid? userId = null)
        {
            var notification = new CompanyHub.Domain.Entities.Notification
            {
                Title = title,
                Message = message,
                IsRead = false,
                CreateAt = DateTime.UtcNow,
                UserId = userId ?? currenttenantService.UserId,
                tenantId = currenttenantService.TenantId,
            };
            context.notification.Add(notification);
            await context.SaveChangesAsync();
            await notificationSender.SendAsync(notification.UserId, title, message);

        }

        public async Task<List<NotificationResponse>> GetAll()
        {
            return await context.notification
                .Where(x=>x.tenantId == currenttenantService.TenantId)
                .OrderByDescending(x=>x.CreateAt)
                .Select(x => new NotificationResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    IsRead = x.IsRead,
                    CreateAt = x.CreateAt
                }).ToListAsync();
        }

        public async Task MarkAsRead(Guid Id)
        {
            var FoundNotification = await context.notification
                .FirstOrDefaultAsync(x=>x.Id == Id && x.tenantId == currenttenantService.TenantId);
            if (FoundNotification != null)
            {
                FoundNotification.IsRead = true;
                await context.SaveChangesAsync();
            }
            else
                throw new Exception("Notification not found");
        }

        public Task<List<NotificationResponse>> Search( SearchNotificationRequest request)
        {
            var query = context.notification
                .Where(x => x.tenantId == currenttenantService.TenantId);
            if(!string.IsNullOrEmpty(request.Title))
            {
                query = query.Where(x => x.Title.Contains(request.Title));
            }
            if (request.IsRead.HasValue)
            {
                query = query.Where(x => x.IsRead == request.IsRead.Value);
            }

            return query.OrderByDescending(x => x.CreateAt)
                .Select(x => new NotificationResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    IsRead = x.IsRead,
                    CreateAt = x.CreateAt
                }).ToListAsync();
        }
    }
}
