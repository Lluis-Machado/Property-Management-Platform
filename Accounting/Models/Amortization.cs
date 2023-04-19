namespace Accounting.Models
{
    public class Amortization
    {
        public Guid FixedAssetId { get; set; }
        public string Period { get; set; }
        public double Amount { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        public Amortization()
        {
            Period = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
