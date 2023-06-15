using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IAPInvoiceLineService
    {
        Task<APInvoiceLineDTO> CreateAPInvoiceLineAsync(CreateAPInvoiceLineDTO createInvoiceLineDTO, Guid invoiceId, string userName);
        Task<IEnumerable<APInvoiceLineDTO>> GetAPInvoiceLinesAsync(bool includeDeleted = false);
        Task<APInvoiceLineDTO> GetAPInvoiceLineByIdAsync(Guid InvoiceLineId);
        Task<APInvoiceLineDTO> UpdateAPInvoiceLineAsync(UpdateAPInvoiceLineDTO updateInvoiceLineDTO, string userName, Guid invoiceLineId, Guid? fixedAssetId = null);
        Task<int> SetDeletedAPInvoiceLineAsync(Guid invoiceLineId, bool deleted);
    }
}
