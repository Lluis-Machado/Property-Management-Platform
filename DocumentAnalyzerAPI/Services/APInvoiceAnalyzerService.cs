using AutoMapper;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAnalyzerAPI.DTOs;
using DocumentAnalyzerAPI.Mappers;

namespace DocumentAnalyzerAPI.Services
{
    public class APInvoiceAnalyzerService: IAPInvoiceAnalyzerService
    {
        private readonly IAzureFormRecognizer _azureFormRecognizer;
        private readonly IAPInvoiceDTOMapper _aPInvoiceDTOMapper;

        public APInvoiceAnalyzerService(IAzureFormRecognizer azureFormRecognizer, IAPInvoiceDTOMapper aPInvoiceDTOMapper)
        {
            _azureFormRecognizer = azureFormRecognizer;
            _aPInvoiceDTOMapper = aPInvoiceDTOMapper;
        }

        public async Task<APInvoiceAnalysisDTO> AnalyzeDocumentAsync(Stream document, string ModelId)
        {
            APInvoiceAnalysisDTO aPInvoiceAnalysisDTO = new();
            AnalyzeResult analyzeResult = await _azureFormRecognizer.AnalyzeDocumentAsync(document, ModelId);
            aPInvoiceAnalysisDTO.Result = analyzeResult;
            foreach (AnalyzedDocument analyzedDocument in analyzeResult.Documents)
            {
                APInvoiceDTO aPInvoiceDTO = _aPInvoiceDTOMapper.MapToAPInvoiceDTO(analyzedDocument.Fields);
                aPInvoiceAnalysisDTO.Invoice = aPInvoiceDTO;
            }
            return aPInvoiceAnalysisDTO;
        }
    }
}
