using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Payment.DTOs
{
    public class PaymobWebhookDto
    {
        public string Type { get; set; } = string.Empty;  // "TRANSACTION"
        public PaymobTransactionDto Obj { get; set; } = null!;
    }

    public class PaymobTransactionDto
    {
        public long Id { get; set; }                    // Transaction ID
        public bool Success { get; set; }               // true = دفع ناجح
        public bool Pending { get; set; }
        public long Amount_Cents { get; set; }          // المبلغ بالقروش
        public string Currency { get; set; } = string.Empty;
        public bool Is_Voided { get; set; }
        public bool Is_Refunded { get; set; }
        public bool Is_Auth { get; set; }
        public bool Is_Capture { get; set; }
        public bool Error_Occured { get; set; }
        public PaymobOrderDto Order { get; set; } = null!;
        public PaymobSourceDataDto Source_Data { get; set; } = null!;
        public string Created_At { get; set; } = string.Empty;
    }

    public class PaymobOrderDto
    {
        public long Id { get; set; }                         // Paymob Order ID
        public string Merchant_Order_Id { get; set; } = string.Empty; // الـ GUID بتاعنا
        public long Amount_Cents { get; set; }
    }

    public class PaymobSourceDataDto
    {
        public string Type { get; set; } = string.Empty;     // "card"
        public string Sub_Type { get; set; } = string.Empty; // "Visa"
        public string Pan { get; set; } = string.Empty;      // "2345" آخر 4 أرقام
    }
}
