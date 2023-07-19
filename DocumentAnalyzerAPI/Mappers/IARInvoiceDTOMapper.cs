using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Mappers
{
    public interface IARInvoiceDTOMapper
    {
        ARInvoiceDTO MapToARInvoiceAndLinesDTO(IReadOnlyDictionary<string, DocumentField> dictionary);
    }
}
