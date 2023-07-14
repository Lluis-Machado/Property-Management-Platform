using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.Contexts;

namespace DocumentAnalyzerAPI.Services
{
    public class AzureFormRecognizer : IAzureFormRecognizer
    {
        private AzureFormRecognizerContext _context { get; set; }

        public AzureFormRecognizer(AzureFormRecognizerContext context)
        {
            _context = context;
        }

        public async Task<AnalyzeResult> AnalyzeDocumentAsync(Stream document, string modelId)
        {
            DocumentAnalysisClient documentAnalysisClient = _context.GetDocumentAnalysisClient();

            AnalyzeDocumentOperation operation = await documentAnalysisClient.AnalyzeDocumentAsync(WaitUntil.Completed, modelId, document);

            AnalyzeResult result = operation.Value;

            return result;
        }
    }
}
