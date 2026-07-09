using CompanyHub.Application.Payment.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Payment
{
    public interface IPaymentService
    {
        Task<Guid> CreatePaymentAsync(CreatepaymentRequest request);
        Task MaskAsAaid(Guid Id);
        Task<List<PaymentResponse>> GetMyPayments();
        Task<InitiatePaymentResponse> InitiatePaymentAsync(InitiatePaymentRequest request);
        Task HandleWebhookAsync(PaymobWebhookDto webhook);
        Task<List<PaymentResponse>> Search(SearchPaymentRequest request);
    }
}
