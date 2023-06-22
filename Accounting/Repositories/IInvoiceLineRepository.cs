using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IInvoiceLineRepository
    {
        Task<InvoiceLine> InsertInvoiceLineAsync(InvoiceLine invoiceLine);
        Task<IEnumerable<InvoiceLine>> GetInvoiceLinesAsync(bool includeDeleted);
        Task<InvoiceLine> GetInvoiceLineByIdAsync(Guid invoiceLineId);
        Task<int> UpdateInvoiceLineAsync(InvoiceLine invoiceLine);
        Task<int> SetDeleteInvoiceLineAsync(Guid id, bool deleted);
    }
}
