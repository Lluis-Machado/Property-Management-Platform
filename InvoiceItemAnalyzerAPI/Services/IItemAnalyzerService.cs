using System.Collections.Generic;
using InvoiceItemAnalyzerAPI.DTOs;

namespace InvoiceItemAnalyzerAPI.Services
{
    public interface IItemAnalyzerService
    {
        IEnumerable<ItemCategoryPredictionDTO> PredictList(List<ItemDTO> invoiceItemDTOs);
    }
}
