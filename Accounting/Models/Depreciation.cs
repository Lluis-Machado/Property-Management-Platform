namespace Accounting.Models
{
    public class Depreciation
    {
        public Guid FixedAssetId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public double Amount { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        public Depreciation()
        {
            LastModificationByUser = string.Empty;
        }
    }
}
