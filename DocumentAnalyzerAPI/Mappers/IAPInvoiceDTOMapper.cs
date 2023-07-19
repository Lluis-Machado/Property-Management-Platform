using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Mappers
{
    public interface IAPInvoiceDTOMapper
    {
        APInvoiceDTO MapToAPInvoiceAndLinesDTO(IReadOnlyDictionary<string, DocumentField> dictionary);
    }
}
