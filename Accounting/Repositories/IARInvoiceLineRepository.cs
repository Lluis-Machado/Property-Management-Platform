using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IARInvoiceLineRepository
    {
        Task<ARInvoiceLine> InsertARInvoiceLineAsync(ARInvoiceLine invoiceLine);
        Task<IEnumerable<ARInvoiceLine>> GetARInvoiceLinesAsync(Guid tenantId, bool includeDeleted = false);
        Task<ARInvoiceLine> GetARInvoiceLineByIdAsync(Guid tenantId, Guid invoiceLineId);
        Task<ARInvoiceLine> UpdateARInvoiceLineAsync(ARInvoiceLine invoiceLine);
        Task<int> SetDeletedARInvoiceLineAsync(Guid invoiceLineId, bool deleted, string userName);
    }
}
