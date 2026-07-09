using CompanyHub.Application.Common.Exceptions;
using CompanyHub.Application.Common.Interfaces;
using CompanyHub.Application.Invoice.DTOs;
using CompanyHub.Application.Notification;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Document = QuestPDF.Fluent.Document;

namespace CompanyHub.Application.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IApplicationDBContext context;
        private readonly INotificationSender notificationSender;
        private readonly ICurrenttenantService currenttenantService;
        public InvoiceService(IApplicationDBContext context,INotificationSender notificationSender,ICurrenttenantService currenttenantService)
        {
            this.context = context;
            this.notificationSender = notificationSender;
            this.currenttenantService = currenttenantService;
        }

        public async Task<Guid> CreateInvoice(Guid PaymenId)
        {
            var payment = await context.payments.FirstOrDefaultAsync(x => x.Id == PaymenId);
            if (payment == null)
                throw new Exception("Payment not found");
            var existingInvoice = await context.invoices.FirstOrDefaultAsync(x => x.PaymentId == PaymenId);
            if (existingInvoice != null)
                throw new BusinessRuleException($"An invoice already exists for this payment (Invoice Number: {existingInvoice.InvoiceNumber}).");
            var issuedAt = DateTime.UtcNow;
            var newInvoice = new Domain.Entities.Invoice
            {
                PaymentId = payment.Id,
                IssuedAt = issuedAt,
                Amount = payment.Amount,
                InvoiceNumber = $"INV-{DateTime.UtcNow.Ticks}",
                IsPaid = true,
                DueDate = issuedAt.AddDays(14)
            };
            context.invoices.Add(newInvoice);
            await context.SaveChangesAsync();
            await notificationSender.SendAsync(currenttenantService.UserId, "Invoice Created", $"<h1>Invoice Created</h1><p>Your invoice {newInvoice.InvoiceNumber} for the payment of {newInvoice.Amount} has been created successfully.</p>");
            return newInvoice.Id;
        }

        public async Task<InvoiceReponse> GetInvoice(Guid Id)
        {
            var invoice = await context.invoices
                .Include(x => x.Payment)
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (invoice == null)
                throw new Exception("Invoice not found");
            return new InvoiceReponse
            {
                InvoiceNumber = invoice.InvoiceNumber,
                Amount = invoice.Amount,
                IssuedAt = invoice.IssuedAt,
                DueDate = invoice.DueDate,
                IsPaid = invoice.IsPaid
            };
        }

        public async Task<byte[]> GetPdf(Guid id)
        {
            var invoice = await context.invoices
                .Include(x => x.Payment)
                .Include(x => x.Payment)
                .ThenInclude(p => p.SubSubscription)
                .ThenInclude(s => s.Plan)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
                throw new Exception("Invoice not found.");

            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);

                    page.DefaultTextStyle(x => x
                        .FontSize(12)
                        .FontFamily("Arial"));

                    // ================= HEADER =================
                    page.Header().Column(header =>
                    {
                        header.Item().Background(Colors.Blue.Darken3)
                            .Padding(20)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text("CompanyHub")
                                    .FontColor(Colors.White)
                                    .FontSize(28)
                                    .Bold();

                                row.ConstantItem(180)
                                    .AlignRight()
                                    .Text("INVOICE")
                                    .FontColor(Colors.White)
                                    .FontSize(26)
                                    .Bold();
                            });

                        header.Item().PaddingTop(15);
                    });

                    // ================= CONTENT =================
                    page.Content().Column(col =>
                    {
                        col.Spacing(18);

                        // Invoice Information
                        col.Item().Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(15)
                            .Column(c =>
                            {
                                c.Item().Text("Invoice Details")
                                    .Bold()
                                    .FontSize(16);

                                c.Item().PaddingTop(10);

                                c.Item().Text($"Invoice Number : {invoice.InvoiceNumber}");
                                c.Item().Text($"Issue Date : {invoice.IssuedAt:dd MMM yyyy}");
                                c.Item().Text($"Due Date : {invoice.DueDate:dd MMM yyyy}");
                            });

                        // Payment Details
                        col.Item().Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(15)
                            .Column(c =>
                            {
                                c.Item().Text("Payment Information")
                                    .Bold()
                                    .FontSize(16);

                                c.Item().PaddingTop(10);

                                c.Item().Text($"Amount : ${invoice.Amount:F2}");

                                if (invoice.Payment != null)
                                {
                                    c.Item().Text($"Payment Status : {invoice.Payment.status}");

                                    c.Item().Text($"Transaction ID : {invoice.Payment.PaymobTransactionId}");

                                    c.Item().Text($"Order ID : {invoice.Payment.PaymobOrderId}");
                                }
                            });

                        // Status Badge
                        col.Item().AlignCenter();

                        col.Item()
                            .Background(invoice.IsPaid
                                ? Colors.Green.Medium
                                : Colors.Orange.Medium)
                            .Padding(10)
                            .AlignCenter()
                            .Text(invoice.IsPaid ? "PAID" : "PENDING")
                            .FontColor(Colors.White)
                            .Bold()
                            .FontSize(16);

                        // Plan Details (اختياري)
                        if (invoice.Payment?.SubSubscription?.Plan != null)
                        {
                            col.Item().Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(15)
                                .Column(c =>
                                {
                                    c.Item().Text("Subscription")
                                        .Bold()
                                        .FontSize(16);

                                    c.Item().PaddingTop(10);

                                    c.Item().Text($"Plan : {invoice.Payment.SubSubscription.Plan.Name}");

                                    c.Item().Text($"Price : ${invoice.Payment.SubSubscription.Plan.Price:F2}");
                                });
                        }
                    });

                    // ================= FOOTER =================
                    page.Footer().Column(footer =>
                    {
                        footer.Item()
                            .LineHorizontal(1)
                            .LineColor(Colors.Grey.Lighten2);

                        footer.Item().PaddingTop(10);

                        footer.Item()
                            .AlignCenter()
                            .Text("Thank you for choosing CompanyHub!")
                            .Bold();

                        footer.Item()
                            .AlignCenter()
                            .Text("This invoice was generated automatically.")
                            .FontSize(10)
                            .FontColor(Colors.Grey.Darken1);

                        footer.Item()
                            .AlignCenter()
                            .Text("support@companyhub.com")
                            .FontSize(10)
                            .FontColor(Colors.Blue.Medium);
                    });
                });
            }).GeneratePdf();
        }

        public Task<List<InvoiceReponse>> SearchInvoices(SearchInvoicerequest request)
        {
            var query = context.invoices.AsQueryable();
            if(request.From.HasValue)
            {
                query = query.Where(x => x.IssuedAt >= request.From.Value);
            }
            if (request.To.HasValue)
            {
                query = query.Where(x => x.IssuedAt <= request.To.Value);
            }
            if(request.MinAmount.HasValue)
            {
                query = query.Where(x => x.Amount >= request.MinAmount.Value);
            }
            if (request.MaxAmount.HasValue)
            {
                query = query.Where(x => x.Amount <= request.MaxAmount.Value);
            }
            if (!string.IsNullOrEmpty(request.InvoiceNumber))
            {
                query = query.Where(x => x.InvoiceNumber.Contains(request.InvoiceNumber));
            }
            if (request.IsPaid.HasValue)
            {
                query = query.Where(x => x.IsPaid == request.IsPaid.Value);
            }

            return query.Select(x => new InvoiceReponse
            {
                InvoiceNumber = x.InvoiceNumber,
                Amount = x.Amount,
                IssuedAt = x.IssuedAt,
                DueDate = x.DueDate,
                IsPaid = x.IsPaid
            }).ToListAsync();
        }
    }
}
