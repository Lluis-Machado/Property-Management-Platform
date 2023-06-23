using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IARInvoiceLineRepository
    {
        Task<ARInvoiceLine> InsertARInvoiceLineAsync(ARInvoiceLine invoiceLine);
        Task<IEnumerable<ARInvoiceLine>> GetARInvoiceLinesAsync(bool includeDeleted = false);
        Task<ARInvoiceLine> GetARInvoiceLineByIdAsync(Guid invoiceLineId);
        Task<ARInvoiceLine> UpdateARInvoiceLineAsync(ARInvoiceLine invoiceLine);
        Task<int> SetDeletedARInvoiceLineAsync(Guid id, bool deleted);
    }
}
