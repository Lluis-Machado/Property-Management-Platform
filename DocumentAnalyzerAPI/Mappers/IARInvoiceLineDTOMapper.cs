using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Mappers
{
    public interface IARInvoiceLineDTOMapper
    {
        ARInvoiceLineDTO MapToARInvoiceLineDTO(IReadOnlyDictionary<string, DocumentField> dictionary);
    }
}
