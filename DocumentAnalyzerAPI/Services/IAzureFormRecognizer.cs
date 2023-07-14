using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DocumentAnalyzerAPI.Services
{
    public interface IAzureFormRecognizer
    {
        Task<AnalyzeResult> AnalyzeDocumentAsync(Stream document, string modelId);
    }
}
