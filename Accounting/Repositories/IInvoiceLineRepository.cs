using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IInvoiceLineRepository
    {
        Task<Guid> InsertInvoiceLineAsync(InvoiceLine invoiceLine);
        Task<IEnumerable<InvoiceLine>> GetInvoiceLinesAsync();
        Task<InvoiceLine> GetInvoiceLineByIdAsync(Guid invoiceLineId);
        Task<int> UpdateInvoiceLineAsync(InvoiceLine invoiceLine);
        Task<int> SetDeleteInvoiceLineAsync(Guid id, bool deleted);
    }
}
