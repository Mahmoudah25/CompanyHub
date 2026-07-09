using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Invoice.DTOs
{
    public class SearchInvoicerequest
    {
        public string? InvoiceNumber { get; set; }
        public bool? IsPaid { get; set; }   
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

    }
}
