using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IAPInvoiceLineService
    {
        Task<APInvoiceLineDTO> CreateAPInvoiceLineAsync(CreateAPInvoiceLineDTO createInvoiceLineDTO, Guid invoiceId, DateTime invoiceDate, string userName);
        Task<IEnumerable<APInvoiceLineDTO>> GetAPInvoiceLinesAsync(bool includeDeleted = false);
        Task<APInvoiceLineDTO> GetAPInvoiceLineByIdAsync(Guid InvoiceLineId);
        Task<APInvoiceLineDTO> UpdateAPInvoiceLineAsync(UpdateAPInvoiceLineDTO updateInvoiceLineDTO, string userName, Guid invoiceLineId, DateTime invoiceDate, Guid? fixedAssetId = null);
        Task<int> SetDeletedAPInvoiceLineAsync(Guid invoiceLineId, bool deleted);
    }
}
