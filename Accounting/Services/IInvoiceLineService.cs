using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IInvoiceLineService
    {
        Task<InvoiceLineDTO> CreateInvoiceLineAsync(CreateInvoiceLineDTO createInvoiceLineDTO, Guid invoiceId, string userName);
        Task<IEnumerable<InvoiceLineDTO>> GetInvoiceLinesAsync(bool includeDeleted = false);
        Task<InvoiceLineDTO> GetInvoiceLineByIdAsync(Guid InvoiceLineId);
        Task<InvoiceLineDTO> UpdateInvoiceLineAsync(UpdateInvoiceLineDTO updateInvoiceLineDTO, string userName, Guid invoiceLineId, Guid? fixedAssetId = null);
        Task<int> SetDeletedInvoiceLineAsync(Guid invoiceLineId, bool deleted);
    }
}
