using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IAPInvoiceRepository
    {
        Task<APInvoice> InsertAPInvoice(APInvoice invoice);
        Task<IEnumerable<APInvoice>> GetAPInvoicesAsync(bool includeDeleted = false);
        Task<APInvoice?> GetAPInvoiceByIdAsync(Guid invoiceId);
        Task<APInvoice> UpdateAPInvoiceAsync(APInvoice invoice);
        Task<int> SetDeletedAPInvoiceAsync(Guid id, bool deleted);
    }
}
