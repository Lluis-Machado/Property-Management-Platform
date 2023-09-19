using InvoiceItemAnalyzerAPI.DTOs;
using System.Collections.Generic;

namespace InvoiceItemAnalyzerAPI.Services
{
    public interface IItemAnalyzerService
    {
        IEnumerable<ItemCategoryPredictionDTO> PredictList(List<ItemDTO> invoiceItemDTOs);
    }
}
