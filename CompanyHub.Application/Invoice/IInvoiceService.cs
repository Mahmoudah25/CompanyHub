using CompanyHub.Application.Invoice.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Invoice
{
    public interface IInvoiceService
    {
        Task<Guid> CreateInvoice(Guid PaymenId);
        Task<InvoiceReponse> GetInvoice(Guid Id);
        Task<byte[]> GetPdf(Guid Id);
        Task<List<InvoiceReponse>> SearchInvoices(SearchInvoicerequest request);    
    }
}
