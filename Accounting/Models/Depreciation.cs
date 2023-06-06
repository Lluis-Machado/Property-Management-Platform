namespace Accounting.Models
{
    public class Depreciation :IAuditable
    {
        public Guid Id { get; set; }
        public Guid FixedAssetId { get; set; }
        public Guid PeriodId { get; set; }
        public double Amount { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }

        public Depreciation()
        {
            LastModificationBy = string.Empty;
        }
    }
}
