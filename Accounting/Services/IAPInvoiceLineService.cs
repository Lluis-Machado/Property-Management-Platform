using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IAPInvoiceLineService
    {
        Task<APInvoiceLineDTO> CreateAPInvoiceLineAsync(Guid tenantId, Guid invoiceId, CreateAPInvoiceLineDTO createInvoiceLineDTO, DateTime invoiceDate, string userName);
        Task<IEnumerable<APInvoiceLineDTO>> GetAPInvoiceLinesAsync(Guid tenantId, bool includeDeleted = false);
        Task<APInvoiceLineDTO> GetAPInvoiceLineByIdAsync(Guid tenantId, Guid InvoiceLineId);
        Task<APInvoiceLineDTO> UpdateAPInvoiceLineAsync(Guid tenantId,Guid invoiceLineId, UpdateAPInvoiceLineDTO updateInvoiceLineDTO, string userName, DateTime invoiceDate, Guid? fixedAssetId = null);
        Task SetDeletedAPInvoiceLineAsync(Guid tenantId, Guid invoiceLineId, bool deleted, string userName);
    }
}
