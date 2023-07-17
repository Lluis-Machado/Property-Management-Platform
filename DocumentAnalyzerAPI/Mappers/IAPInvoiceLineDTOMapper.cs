using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Mappers
{
    public interface IAPInvoiceLineDTOMapper
    {
        APInvoiceLineDTO MapToAPInvoiceLineDTO(IReadOnlyDictionary<string, DocumentField> dictionary);
    }
}
