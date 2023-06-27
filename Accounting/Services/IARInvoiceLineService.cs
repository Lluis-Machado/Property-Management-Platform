using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IARInvoiceLineService
    {
        Task<ARInvoiceLineDTO> CreateARInvoiceLineAsync(Guid tenantId, Guid invoiceId, CreateARInvoiceLineDTO createInvoiceLineDTO, string userName);
        Task<IEnumerable<ARInvoiceLineDTO>> GetARInvoiceLinesAsync(Guid tenantId, bool includeDeleted = false);
        Task<ARInvoiceLineDTO> GetARInvoiceLineByIdAsync(Guid tenantId, Guid InvoiceLineId);
        Task<ARInvoiceLineDTO> UpdateARInvoiceLineAsync(Guid tenantId, Guid invoiceLineId, UpdateARInvoiceLineDTO updateInvoiceLineDTO, string? userName, Guid? fixedAssetId = null);
        Task SetDeletedARInvoiceLineAsync(Guid tenantId, Guid invoiceLineId, bool deleted, string userName);
        Task<List<DateTime>> GetListOfServiceDatesInPeriodAsync(Guid tenantId, DateTime dateFrom, DateTime dateTo);
    }
}
