using CompanyHub.Application.Invoice;
using CompanyHub.Application.Invoice.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyHub.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService invoiceService;
        public InvoiceController(IInvoiceService invoiceService)
        {
            this.invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(Guid PaymentId)
        {
            var invoiceId = await invoiceService.CreateInvoice(PaymentId);
            return Ok(invoiceId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var invoice = await invoiceService.GetInvoice(id);
            return Ok(invoice);
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetInvoicePdf(Guid id)
        {
            var pdfBytes = await invoiceService.GetPdf(id);
            return File(pdfBytes, "application/pdf", $"invoice_{id}.pdf");
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchInvoices([FromQuery] SearchInvoicerequest request)
        {
            var invoices = await invoiceService.SearchInvoices(request);
            return Ok(invoices);
        }
    }
}
