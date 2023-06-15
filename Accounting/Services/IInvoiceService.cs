using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDTO> CreateInvoiceAndLinesAsync(CreateInvoiceDTO createInvoiceDTO, string userName, Guid businessPartnerId);
        Task<IEnumerable<InvoiceDTO>> GetInvoicesAsync(bool includeDeleted = false);
        Task<bool> CheckIfInvoiceExistsAsync(Guid invoiceId);
        Task<InvoiceDTO> UpdateInvoiceAndLinesAsync(UpdateInvoiceDTO updateInvoiceDTO, string userName, Guid invoiceId);
        Task<int> SetDeleteInvoiceAsync(Guid invoiceId, bool deleted);
    }
}
