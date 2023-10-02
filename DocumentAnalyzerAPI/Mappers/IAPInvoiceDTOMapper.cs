using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using AccountingAPI.DTOs;


namespace DocumentAnalyzerAPI.Mappers
{
    public interface IAPInvoiceDTOMapper
    {
        APInvoiceDTO MapToAPInvoiceAndLinesDTO(IReadOnlyDictionary<string, DocumentField> dictionary);
    }
}
