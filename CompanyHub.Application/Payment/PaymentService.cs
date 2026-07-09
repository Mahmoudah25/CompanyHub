using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Invoice;
using CompanyHub.Application.Notification;
using CompanyHub.Application.Payment.DTOs;
using CompanyHub.Domain.Entities;
using CompanyHub.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IApplicationDBContext context;
        private readonly IEmailService emailService;
        private readonly ICurrenttenantService currenttenant;
        private readonly IPaymentGateway paymentGateaway;
        private readonly IInvoiceService invoiceService;
        private readonly INotificationSender notificationSender;
        public PaymentService
            (IApplicationDBContext context,IEmailService emailService,
            ICurrenttenantService currenttenant,IPaymentGateway paymentGateaway
            ,IInvoiceService invoiceService,INotificationSender notificationSender)
        {
            this.context = context;
            this.emailService = emailService;
            this.currenttenant = currenttenant;
            this.paymentGateaway = paymentGateaway;
            this.invoiceService = invoiceService;
            this.notificationSender = notificationSender;
        }

        public async Task<Guid> CreatePaymentAsync(CreatepaymentRequest request)
        {
            var SubscrtionFound = await context.subscriptions
                .Include(x=>x.Plan)
                .Include(x=>x.Tenants)
                .FirstOrDefaultAsync(x => x.Id == request.SubscriptionId);
            if (SubscrtionFound == null)
                throw new Exception(" Plan Not Found ");
            var user = await context.users.FirstOrDefaultAsync(x => x.TenantId == SubscrtionFound.TenantId);
            var payment = new CompanyHub.Domain.Entities.Payment
            {
                Amount= SubscrtionFound.Plan.Price,
                status = Domain.Enum.PaymentStatus.Pending,
                PaymentDate= DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString(),
                SubStractionId= request.SubscriptionId
            };
            context.payments.Add(payment);
            await context.SaveChangesAsync();
            await emailService.SendEmail(user.Email, "Payment Created", $"<h1>Payment Created</h1><p>Your payment of {payment.Amount} has been created successfully. Transaction ID: {payment.TransactionId}</p>");
            return payment.Id;

        }

        public async Task<List<PaymentResponse>> GetMyPayments()
        {
            return
                await context.payments
                .Select( x=> new PaymentResponse
                {
                    Id = x.Id,
                    Amount= x.Amount,
                    Status = x.status.ToString(),
                    PaymentDate= x.PaymentDate,
                }
                ).ToListAsync();
            
        }

        public async Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request)
        {
            var subscription = await context.subscriptions.Include(s=>s.Plan)
                .FirstOrDefaultAsync(s=>s.Id == request.SubscriptionId && s.TenantId == currenttenant.TenantId);
            if (subscription == null)
                throw new Exception(" subscription Not Found");
            var user = await context.users.FirstOrDefaultAsync(x => x.TenantId == currenttenant.TenantId);
            if (user == null)
                throw new Exception(" User Not Found ");
            var authtoken = await paymentGateaway.GetAuthTokenAsync();
            var Payment = new CompanyHub.Domain.Entities.Payment
            {
                SubStractionId = subscription.Id,
                Amount = subscription.Plan.Price,
                status = PaymentStatus.Pending,
            };
            context.payments.Add(Payment);
            await context.SaveChangesAsync();
            var paymentOrderId = await paymentGateaway.CreateOrderAsync(authtoken,Payment.Amount,Payment.Id.ToString());    
            var paymentKey = await paymentGateaway.GetPaymentKeyAsync(authtoken,paymentOrderId,Payment.Amount,user.Email);
            Payment.PaymobOrderId = paymentOrderId;
            Payment.PaymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{paymentGateaway.IFrameId}?payment_token={paymentKey}";
            await context.SaveChangesAsync();
            return new InitiatePaymentResponse
            {
                PaymentId = Payment.Id,
                PaymentUrl = Payment.PaymentUrl
            };

        }

        public async Task MaskAsAaid(Guid Id)
        {
            var payment = await context.payments
                                .FirstOrDefaultAsync(x => x.Id == Id);

            if (payment == null)
                throw new Exception("Payment Not Found");

            payment.status = PaymentStatus.paid;
            await invoiceService.CreateInvoice(payment.Id);

            var subscription =
                await context.subscriptions
                .FirstOrDefaultAsync(x =>
                    x.Id == payment.SubStractionId);

            if (subscription != null)
            {
                subscription.IsActive = true;
            }

            await context.SaveChangesAsync();
        }


        async Task IPaymentService.HandleWebhookAsync(PaymobWebhookDto webhook)
        {
            var payment = await context.payments
                      .IgnoreQueryFilters()
                      .FirstOrDefaultAsync(p =>
                       p.Id.ToString() == webhook.Obj.Order.Merchant_Order_Id);
  
            if (payment is null) return;

            payment.status = webhook.Obj.Success ? PaymentStatus.paid : PaymentStatus.failed;
            payment.PaymobTransactionId = webhook.Obj.Id.ToString();

            await invoiceService.CreateInvoice(payment.Id);

            if (webhook.Obj.Success)
            {
                var subscription = await context.subscriptions
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(s => s.Id == payment.SubStractionId);

                if (subscription is not null)
                {
                    subscription.IsActive = true;
                    subscription.StartDate = DateTime.UtcNow;
                    subscription.EndDate = DateTime.UtcNow.AddMonths(1);
                }
            }

            await context.SaveChangesAsync();
            await notificationSender.SendAsync(currenttenant.UserId, "Payment Status Updated", $"Your payment with ID {payment.Id} has been updated to status: {payment.status}.");
        }
        public async Task<List<PaymentResponse>> Search(SearchPaymentRequest request)
        {
            var query = context.payments.AsQueryable(); 
            if(request.Status.HasValue)
                query = query.Where(p => p.status == request.Status.Value);
            if(request.From.HasValue)
                query = query.Where(p => p.CreateAt >= request.From);
            if (request.To.HasValue)
                query = query.Where(p => p.CreateAt <= request.To);
            if(request.MinAmount.HasValue)
                query = query.Where(p => p.Amount >= request.MinAmount.Value);
            if (request.MaxAmount.HasValue)
                query = query.Where(p => p.Amount <= request.MaxAmount.Value);
            return await query.Select(p => new PaymentResponse
            {
                Id = p.Id,
                Amount = p.Amount,
                Status = p.status.ToString(),
                PaymentDate = p.PaymentDate,
            }).ToListAsync();
        }
    }
}
