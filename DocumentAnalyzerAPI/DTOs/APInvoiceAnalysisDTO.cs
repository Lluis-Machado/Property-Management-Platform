using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DocumentAnalyzerAPI.DTOs
{
    public class APInvoiceAnalysisDTO
    {
        public APInvoiceDTO invoice { get; set; }
        public AnalyzeResult result { get; set; }
    }
}
