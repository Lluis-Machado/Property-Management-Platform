using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice> InsertInvoice(Invoice invoice);
        Task<IEnumerable<Invoice>> GetInvoicesAsync(bool includeDeleted = false);
        Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId);
        Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
        Task<int> SetDeleteInvoiceAsync(Guid id, bool deleted);
    }
}
