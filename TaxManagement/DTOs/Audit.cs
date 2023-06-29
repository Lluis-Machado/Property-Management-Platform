namespace TaxManagementAPI.DTOs
{
    public class Audit
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public Object? OldObject { get; set; }
        public Object? NewObject { get; set; }
        public DateTime Timestamp { get; set; }
        public string ChangedBy { get; set; } = string.Empty;


    }
}
