using CompanyHub.Application.Report.DTOs;
using CompanyHub.Application.Subscription.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Report
{
    public interface IReportService
    {
        Task<RevenueReportResponse> GetRevenueReportAsync(RevenueReportRequest request);
        Task<SubscriptionsOverviewResponsive> GetSubscriptionsOverviewAsync();
    }
}
