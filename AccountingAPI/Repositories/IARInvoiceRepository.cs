using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IARInvoiceRepository
    {
        Task<ARInvoice> InsertARInvoice(ARInvoice invoice);
        Task<IEnumerable<ARInvoice>> GetARInvoicesAsync(Guid tenantId, bool includeDeleted = false);
        Task<ARInvoice?> GetARInvoiceByIdAsync(Guid tenantId, Guid invoiceId);
        Task<ARInvoice> UpdateARInvoiceAsync(ARInvoice invoice);
        Task<int> SetDeletedARInvoiceAsync(Guid invoiceId, bool deleted, string userName);
    }
}
