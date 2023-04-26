using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Guid> InsertInvoiceAsync(Invoice invoice);
        Task <IEnumerable<Invoice>> GetInvoicesAsync();
        Task<Invoice> GetInvoiceByIdAsync(Guid invoiceId);
        Task<int> UpdateInvoiceAsync(Invoice invoice);
        Task<int> SetDeleteInvoiceAsync(Guid id, bool deleted); 
    }
}
