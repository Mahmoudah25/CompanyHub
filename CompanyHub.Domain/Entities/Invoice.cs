using CompanyHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime IssuedAt { set; get; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public Guid PaymentId { get; set; }
        public Payment Payment { get; set; } = null!;
    }
}
