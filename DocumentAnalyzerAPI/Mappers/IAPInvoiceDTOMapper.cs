using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Mappers
{
    public interface IAPInvoiceDTOMapper
    {
        APInvoiceDTO MapToAPInvoiceDTO(IReadOnlyDictionary<string, DocumentField> dictionary);
    }
}
