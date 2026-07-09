using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Report.DTOs
{
    public class RevenueReportResponse
    {
        public List<RevenueReportItem> Items { get; set; } = new();
        public decimal GrandTotal { get; set; }
    }
}
