using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace DocumentAnalyzerAPI.DTOs
{
    public class DocumentAnalysisDTO<T>
    {
        public T? Form { get; set; }
        public AnalyzeResult? AnalyzeResult { get; set; }
    }

}
