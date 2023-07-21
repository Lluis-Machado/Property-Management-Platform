namespace InvoiceItemAnalyzerAPI.DTOs
{
    public class ItemCategoryPredictionDTO
    {
        public int PredictedCategoryId { get; set; }
        public float Confidence { get; set; }
    }
}
