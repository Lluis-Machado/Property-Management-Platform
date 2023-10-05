using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using AccountingAPI.DTOs;

namespace DocumentAnalyzerAPI.Mappers
{
    public interface IAPInvoiceLineDTOMapper
    {
        APInvoiceLineDTO MapToAPInvoiceLineDTO(IReadOnlyDictionary<string, DocumentField> dictionary);
    }
}
