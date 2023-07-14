using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IAPInvoiceRepository
    {
        Task<APInvoice> InsertAPInvoice(APInvoice invoice);
        Task<IEnumerable<APInvoice>> GetAPInvoicesAsync(Guid tenantId, bool includeDeleted = false);
        Task<APInvoice?> GetAPInvoiceByIdAsync(Guid tenantId, Guid invoiceId);
        Task<APInvoice> UpdateAPInvoiceAsync(APInvoice invoice);
        Task<int> SetDeletedAPInvoiceAsync(Guid id, bool deleted, string userName);
    }
}
