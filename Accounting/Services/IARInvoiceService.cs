using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IARInvoiceService
    {
        Task<ARInvoiceDTO> CreateARInvoiceAndLinesAsync(Guid tenantId, Guid businessPartnerId, CreateARInvoiceDTO createInvoiceDTO, string userName);     
        Task<IEnumerable<ARInvoiceDTO>> GetARInvoicesAsync(Guid tenantId, bool includeDeleted = false);
        Task<ARInvoiceDTO> UpdateARInvoiceAndLinesAsync(Guid tenantId, Guid invoiceId, UpdateARInvoiceDTO updateInvoiceDTO, string userName);
        Task SetDeletedARInvoiceAsync(Guid tenantId, Guid invoiceId, bool deleted, string userName);
    }
}
