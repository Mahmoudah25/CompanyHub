using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Invoice.DTOs
{
    public class InvoiceReponse
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime IssuedAt { set; get; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }

    }
}
