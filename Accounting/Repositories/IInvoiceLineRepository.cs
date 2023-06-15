using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IInvoiceLineRepository
    {
        Task<InvoiceLine> InsertInvoiceLineAsync(InvoiceLine invoiceLine);
        Task<IEnumerable<InvoiceLine>> GetInvoiceLinesAsync(bool includeDeleted);
        Task<InvoiceLine> GetInvoiceLineByIdAsync(Guid invoiceLineId);
        Task<InvoiceLine> UpdateInvoiceLineAsync(InvoiceLine invoiceLine);
        Task<int> SetDeletedInvoiceLineAsync(Guid id, bool deleted);
    }
}
