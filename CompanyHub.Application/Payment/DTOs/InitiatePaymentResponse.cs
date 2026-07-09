using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Payment.DTOs
{
    public class InitiatePaymentResponse
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
    }
}
