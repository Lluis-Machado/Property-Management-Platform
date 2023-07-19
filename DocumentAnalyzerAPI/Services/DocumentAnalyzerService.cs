﻿using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Mappers;
using static DocumentAnalyzerAPI.Utilities.Document;

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

        public async Task<DocumentAnalysisDTO<T>> AnalyzeDocumentAsync<T>(Stream document) where T: Enum
        {
            DocumentAnalysisDTO<T> documentAnalysisDTO = new();
            string modelId = GetModelIdForEnum<T>(); // Get the modelId based on T
            AnalyzeResult analyzeResult = await _azureFormRecognizer.AnalyzeDocumentAsync(document, modelId);
            documentAnalysisDTO.Form = await _documentFieldsMapper.Map<T>(analyzeResult.Documents[0].Fields);
            return documentAnalysisDTO;
        }

        private string GetModelIdForEnum<T>() where T : Enum
        {
            // Implement your logic to map the generic type T to a specific modelId
            // For example, using a dictionary:
            Dictionary<Type, string> modelIdMapping = new()
            {
                { typeof(APInvoiceDTO), "prebuilt-invoice" },
                // Add more entries as needed
            };

            if (modelIdMapping.TryGetValue(typeof(T), out string modelId))
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
