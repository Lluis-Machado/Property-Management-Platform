using DocumentAnalyzerAPI.DTOs;

namespace DocumentAnalyzerAPI.Services
{
    public class DocumentAnalyzerService: IDocumentAnalyzerService
    {
        private readonly IAzureFormRecognizer _azureFormRecognizer;

        public DocumentAnalyzerService(IAzureFormRecognizer azureFormRecognizer)
        {
            _azureFormRecognizer = azureFormRecognizer;
        }

        public async Task<DocumentAnalysisDTO> AnalyzeDocumentAsync(Stream document, string ModelId)
        {
            DocumentAnalysisDTO documentAnalysisDTO = new();
            documentAnalysisDTO.result = await _azureFormRecognizer.AnalyzeDocumentAsync(document, ModelId);
            return documentAnalysisDTO;
        }
    }
}
