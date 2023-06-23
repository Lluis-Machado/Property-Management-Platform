using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IARInvoiceService
    {
        Task<ARInvoiceDTO> CreateARInvoiceAndLinesAsync(CreateARInvoiceDTO createInvoiceDTO, string? userName, Guid businessPartnerId);
        //Task<IEnumerable<ARInvoiceDTO>> GetARInvoicesAsync(Guid tenantId, bool includeDeleted = false);
        Task<bool> CheckIfARInvoiceExistsAsync(Guid invoiceId);
        Task<ARInvoiceDTO> UpdateARInvoiceAndLinesAsync(UpdateARInvoiceDTO updateInvoiceDTO, string? userName, Guid invoiceId);
        Task<int> SetDeletedARInvoiceAsync(Guid invoiceId, bool deleted);
    }
}
