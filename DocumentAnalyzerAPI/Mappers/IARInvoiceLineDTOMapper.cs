using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using AccountingAPI.DTOs;


namespace DocumentAnalyzerAPI.Mappers
{
    public interface IARInvoiceLineDTOMapper
    {
        ARInvoiceLineDTO MapToARInvoiceLineDTO(IReadOnlyDictionary<string, DocumentField> dictionary);
    }
}
