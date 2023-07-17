using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DocumentAnalyzerAPI.DTOs
{
    public class APInvoiceAnalysisDTO
    {
        public APInvoiceDTO Invoice { get; set; }
        public AnalyzeResult Result { get; set; }
    }
}
