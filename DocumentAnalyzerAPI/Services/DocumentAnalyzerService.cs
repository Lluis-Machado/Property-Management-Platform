using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Mappers;
using System.Text.RegularExpressions;

namespace DocumentAnalyzerAPI.Services
{
    public class DocumentAnalyzerService: IDocumentAnalyzerService
    {
        private readonly IAzureFormRecognizer _azureFormRecognizer;
        private readonly IDocumentFieldsMapper _documentFieldsMapper;

        public DocumentAnalyzerService(IAzureFormRecognizer azureFormRecognizer, IDocumentFieldsMapper documentFieldsMapper)
        {
            _azureFormRecognizer = azureFormRecognizer;
            _documentFieldsMapper = documentFieldsMapper;
        }

        public async Task<DocumentAnalysisDTO<T>> AnalyzeDocumentAsync<T>(Stream document)
        {
            DocumentAnalysisDTO<T> documentAnalysisDTO = new();
            string modelId = GetModelIdForEnum<T>(); // Get the modelId based on T
            AnalyzeResult analyzeResult = await _azureFormRecognizer.AnalyzeDocumentAsync(document, modelId);
            documentAnalysisDTO.AnalyzeResult = analyzeResult;
            documentAnalysisDTO.Form = _documentFieldsMapper.Map<T>(analyzeResult.Documents[0].Fields);
            var cif = FindString(analyzeResult.Content);
            documentAnalysisDTO.CIF = cif;
            return documentAnalysisDTO;
        }

        public static string FindString(string input)
        {
            // The regex pattern:
            // ^[a-e] - Starts with a or b or c or d or e
            // [0-9]{8}  - Followed by 8 numbers (total length = 9)
            string pattern = @"[A-E](?<!\d)[0-9]{8}(?!\d)";

            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                return match.Value;
            }
            return null;
        }

        private static string GetModelIdForEnum<T>()
        {
            // Implement your logic to map the generic type T to a specific modelId
            // For example, using a dictionary:
            Dictionary<Type, string> modelIdMapping = new()
            {
                { typeof(APInvoiceDTO), "prebuilt-invoice" },
                { typeof(ARInvoiceDTO), "prebuilt-invoice" },
                // Add more entries as needed
            };

            if (modelIdMapping.TryGetValue(typeof(T), out var modelId))
            {
                return modelId;
            }
            else
            {
                // Handle the case when the modelId for the given enum type T is not found
                throw new InvalidOperationException($"ModelId not found for enum type {typeof(T).FullName}.");
            }
        }
    }
}
