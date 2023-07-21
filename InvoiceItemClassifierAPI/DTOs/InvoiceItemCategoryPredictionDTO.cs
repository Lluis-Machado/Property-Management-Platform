namespace InvoiceItemClassifierAPI.DTOs
{
    public class InvoiceItemCategoryPredictionDTO
    {
        public int PredictedCategoryId { get; set; }
        public float Confidence { get; set; }
    }
}
