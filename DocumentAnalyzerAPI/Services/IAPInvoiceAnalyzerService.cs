using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Services
{
    public interface IAPInvoiceAnalyzerService
    {
        Task<APInvoiceAnalysisDTO> AnalyzeDocumentAsync(Stream document, string ModelId);
    }
}
