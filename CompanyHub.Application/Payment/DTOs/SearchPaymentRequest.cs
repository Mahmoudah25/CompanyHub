using CompanyHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Payment.DTOs
{
    public class SearchPaymentRequest
    {
        public PaymentStatus? Status { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? From { get; set; } 
        public DateTime? To { get; set; }

    }
}
