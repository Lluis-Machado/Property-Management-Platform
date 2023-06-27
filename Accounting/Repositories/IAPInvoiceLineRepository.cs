using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IAPInvoiceLineRepository
    {
        Task<APInvoiceLine> InsertAPInvoiceLineAsync(APInvoiceLine invoiceLine);
        Task<IEnumerable<APInvoiceLine>> GetAPInvoiceLinesAsync(Guid tenandId, bool includeDeleted = false);
        Task<APInvoiceLine> GetAPInvoiceLineByIdAsync(Guid tenantId, Guid invoiceLineId);
        Task<APInvoiceLine> UpdateAPInvoiceLineAsync(APInvoiceLine invoiceLine);
        Task<int> SetDeletedAPInvoiceLineAsync(Guid id, bool deleted, string userName);
    }
}
