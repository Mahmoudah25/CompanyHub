using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Report.DTOs
{
    public class RevenueReportRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string GroupBy { get; set; } = "month"; // day, week, month, year
    }
}
