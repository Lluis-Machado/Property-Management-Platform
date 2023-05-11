namespace TaxManagementAPI.Models
{
    public interface IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string? CreatedByUser { get; set; }
        public string? LastUpdateByUser { get; set; }
    }
}
