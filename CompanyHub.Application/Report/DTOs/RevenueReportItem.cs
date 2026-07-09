using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Report.DTOs
{
    public class RevenueReportItem
    {
        public string Period { get; set; } = string.Empty; // "2026-07" أو "2026-07-04"
        public decimal TotalRevenue { get; set; }
        public int TransactionsCount { get; set; }
    }
}
