namespace Accounting.Models
{
    public interface IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }
    }
}
