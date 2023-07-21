using static InvoiceItemClassifierAPI.InvoiceLine;
using System.Collections.Generic;
using InvoiceItemClassifierAPI.DTOs;

namespace InvoiceItemClassifierAPI.Services
{
    public interface IItemClassifierService
    {
        IEnumerable<InvoiceItemCategoryPredictionDTO> PredictList(List<InvoiceItemDTO> invoiceItemDTOs);
    }
}
