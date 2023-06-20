namespace AccountingAPI.DTOs
{
    public class DepreciationDTO
    {
        public Guid Id { get; set; }
        public Guid FixedAssetId { get; set; }
        public Guid PeriodId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }  
        public double DepreciationAmount { get; set; }
    }
}
