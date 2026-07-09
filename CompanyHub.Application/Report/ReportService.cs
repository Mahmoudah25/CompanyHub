using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Report.DTOs;
using CompanyHub.Application.Subscription.DTOs;
using CompanyHub.Domain.Entities;
using CompanyHub.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Report
{
    public class ReportService : IReportService
    {
        private readonly IApplicationDBContext context;
        private readonly ICurrenttenantService currenttenantService;
        private readonly ICacheService cacheService;
        private readonly ILogger<ReportService> logger;
        public ReportService(IApplicationDBContext context, ICurrenttenantService currenttenantService,ICacheService cacheService, ILogger<ReportService> logger)
        {
            this.context = context;
            this.currenttenantService = currenttenantService;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<RevenueReportResponse> GetRevenueReportAsync(RevenueReportRequest request)
        {
            try
            {
                var cachKey = $"RenevueReport:{currenttenantService.TenantId}:{request.FromDate}:{request.ToDate}";
                try
                {
                    var cashed = await cacheService.GetAsync<RevenueReportResponse>(cachKey);
                    if (cashed != null)
                        return cashed;
                }
                catch(Exception casheEx)
                {
                    logger.LogWarning(casheEx, "Cache read failed for RevenueReport, falling back to DB.");
                }

                var query = context.payments
                    .Include(p => p.SubSubscription)
                    .Where(p => p.SubSubscription.TenantId == currenttenantService.TenantId && p.status == PaymentStatus.paid);
                    
                if (request.FromDate.HasValue)
                    query = query.Where(p => p.CreateAt >= request.FromDate.Value);
                if (request.ToDate.HasValue)
                    query = query.Where(p => p.CreateAt <= request.ToDate.Value);

                List<RevenueReportItem> items;

                if (request.GroupBy == "day")
                {
                    items = await query
                        .GroupBy(p => new { p.CreateAt.Year, p.CreateAt.Month, p.CreateAt.Day })
                        .Select(g => new RevenueReportItem
                        {
                            Period = g.Key.Year + "-" + g.Key.Month.ToString("00") + "-" + g.Key.Day.ToString("00"),
                            TotalRevenue = g.Sum(p => p.Amount),
                            TransactionsCount = g.Count()
                        })
                        .OrderBy(i => i.Period)
                        .ToListAsync();
                }
                else if (request.GroupBy == "year")
                {
                    items = await query
                        .GroupBy(p => p.CreateAt.Year)
                        .Select(g => new RevenueReportItem
                        {
                            Period = g.Key.ToString(),
                            TotalRevenue = g.Sum(p => p.Amount),
                            TransactionsCount = g.Count()
                        })
                        .OrderBy(i => i.Period)
                        .ToListAsync();
                }
                else // month (default)
                {
                    items = await query
                        .GroupBy(p => new { p.CreateAt.Year, p.CreateAt.Month })
                        .Select(g => new RevenueReportItem
                        {
                            Period = g.Key.Year + "-" + g.Key.Month.ToString("00"),
                            TotalRevenue = g.Sum(p => p.Amount),
                            TransactionsCount = g.Count()
                        })
                        .OrderBy(i => i.Period)
                        .ToListAsync();
                }

                var response = new RevenueReportResponse
                {
                    Items = items,
                    GrandTotal = items.Sum(i => i.TotalRevenue)
                };

                try
                {
                    await cacheService.SetAsync(cachKey, response, TimeSpan.FromMinutes(10));
                }
                catch (Exception cacheEx)
                {
                    logger.LogWarning(cacheEx, "Cache write failed for RevenueReport.");
                }

                return response;

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRevenueReportAsync failed for TenantId {TenantId}", currenttenantService.TenantId);
                // رجّع Response فاضي بدل ما توقع الـ API بالكامل
                return new RevenueReportResponse
                {
                    Items = new List<RevenueReportItem>(),
                    GrandTotal = 0
                };
            }
        }

        public async Task<SubscriptionsOverviewResponsive> GetSubscriptionsOverviewAsync()
        {
            try
            {
                var cahedKey = $"SubscriptionsOverview:{currenttenantService.TenantId}";
                try
                {
                    var cashed = await cacheService.GetAsync<SubscriptionsOverviewResponsive>(cahedKey);
                    if (cashed != null)
                        return cashed;
                }
                catch (Exception ex) 
                {
                    logger.LogWarning(ex, "Cache read failed for SubscriptionsOverview, falling back to DB.");
                }
                var subscription = await context.subscriptions
                    .Include(s => s.Plan)
                    .Where(s => s.TenantId == currenttenantService.TenantId)
                    .ToListAsync();
                var response = new SubscriptionsOverviewResponsive
                {
                    ActiveCount = subscription.Count(s => s.IsActive),
                    CancelledCount = subscription.Count(s => !s.IsActive),
                    ByPlan = subscription
                    .GroupBy(s => s.Plan?.Name ?? "UnKnown")
                    .ToDictionary(g => g.Key, g => g.Count())
                };
                try
                {
                    await cacheService.SetAsync(cahedKey, response, TimeSpan.FromMinutes(10));
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Cache write failed for SubscriptionsOverview.");
                }
                return response;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "GetSubscriptionsOverviewAsync failed for TenantId {TenantId}", currenttenantService.TenantId);
                return new SubscriptionsOverviewResponsive
                {
                    ActiveCount = 0,
                    CancelledCount = 0,
                    ByPlan = new Dictionary<string, int>()
                };

            }
        }
    }
}
