namespace InvoiceItemAnalyzerAPI.DTOs
{
    public class ItemCategoryPredictionDTO
    {
        public string PredictedCategoryId { get; set; }
        public float Confidence { get; set; }
    }
}
