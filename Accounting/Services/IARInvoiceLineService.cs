using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IARInvoiceLineService
    {
        Task<ARInvoiceLineDTO> CreateARInvoiceLineAsync(CreateARInvoiceLineDTO createInvoiceLineDTO, Guid invoiceId, string userName);
        Task<IEnumerable<ARInvoiceLineDTO>> GetARInvoiceLinesAsync(bool includeDeleted = false);
        Task<ARInvoiceLineDTO> GetARInvoiceLineByIdAsync(Guid InvoiceLineId);
        Task<ARInvoiceLineDTO> UpdateARInvoiceLineAsync(UpdateARInvoiceLineDTO updateInvoiceLineDTO, string userName, Guid invoiceLineId, Guid? fixedAssetId = null);
        Task<int> SetDeletedARInvoiceLineAsync(Guid invoiceLineId, bool deleted);
        Task<List<DateTime>> GetListOfServiceDatesInPeriodAsync(DateTime dateFrom, DateTime dateTo);
    }
}
