using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IAPInvoiceService
    {
        Task<APInvoiceDTO> CreateAPInvoiceAndLinesAsync(CreateAPInvoiceDTO createInvoiceDTO, string userName, Guid businessPartnerId);
        Task<IEnumerable<APInvoiceDTO>> GetAPInvoicesAsync(bool includeDeleted = false);
        Task<bool> CheckIfAPInvoiceExistsAsync(Guid invoiceId);
        Task<APInvoiceDTO> UpdateAPInvoiceAndLinesAsync(UpdateAPInvoiceDTO updateInvoiceDTO, string userName, Guid invoiceId);
        Task<int> SetDeletedAPInvoiceAsync(Guid invoiceId, bool deleted);
    }
}
