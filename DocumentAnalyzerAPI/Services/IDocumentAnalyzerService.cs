using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Services
{
    public interface IDocumentAnalyzerService
    {
        Task<DocumentAnalysisDTO<T>> AnalyzeDocumentAsync<T>(Stream document);
    }
}
