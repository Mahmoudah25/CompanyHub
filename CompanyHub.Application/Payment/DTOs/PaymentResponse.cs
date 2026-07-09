using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Payment.DTOs
{
    public class PaymentResponse
    {
        public Guid Id { set; get; }
        public decimal Amount {  set; get; }
        public string Status { set; get; } = string.Empty;
        public DateTime PaymentDate { set; get; }
    }
}
