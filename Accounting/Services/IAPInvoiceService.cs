using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IAPInvoiceService
    {
        Task<APInvoiceDTO> CreateAPInvoiceAndLinesAsync(Guid tenantId, Guid businessPartnerId, CreateAPInvoiceDTO createInvoiceDTO, string userName);
        Task<IEnumerable<APInvoiceDTO>> GetAPInvoicesAsync(Guid tenantId, bool includeDeleted = false, int? page = null, int? pageSize= null);
        Task<APInvoiceDTO> UpdateAPInvoiceAndLinesAsync(Guid tenantId, Guid invoiceId, UpdateAPInvoiceDTO updateInvoiceDTO, string userName);
        Task SetDeletedAPInvoiceAsync(Guid tenantId, Guid invoiceId, bool deleted, string userName);
    }
}
