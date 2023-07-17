using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Services
{
    public interface IDocumentAnalyzerService
    {
        Task<DocumentAnalysisDTO> AnalyzeDocumentAsync(Stream document, string ModelId);
    }
}
