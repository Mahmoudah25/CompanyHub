using CompanyHub.Domain.Common;
using CompanyHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public decimal  Amount { get; set; }
        public PaymentStatus status { get; set; } 
        public string TransactionId { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }

        // Paymob fields
        public string? PaymobOrderId { get; set; }
        public string? PaymobTransactionId { get; set; }
        public string? PaymentUrl { get; set; }
        // FK
        [ForeignKey("SubSubscription")]
        public Guid SubStractionId { get; set; }
        public Subscription SubSubscription { get; set; } = null!;
        public Invoice Invoice { get; set; } = null!;
    }
}
