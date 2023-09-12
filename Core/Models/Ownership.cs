namespace CoreAPI.Models
{
    public class Ownership
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerType { get; set; } = "";
    }
}
