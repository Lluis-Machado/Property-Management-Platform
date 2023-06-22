using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IARInvoiceRepository
    {
        Task<ARInvoice> InsertARInvoice(ARInvoice invoice);
        Task<IEnumerable<ARInvoice>> GetARInvoicesAsync(bool includeDeleted = false);
        Task<ARInvoice?> GetARInvoiceByIdAsync(Guid invoiceId);
        Task<ARInvoice> UpdateARInvoiceAsync(ARInvoice invoice);
        Task<int> SetDeletedARInvoiceAsync(Guid id, bool deleted);
    }
}
