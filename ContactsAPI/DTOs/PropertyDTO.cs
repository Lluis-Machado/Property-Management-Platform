namespace ContactsAPI.DTOs
{
    public class PropertyDTO
    {
        public Guid Guid { get; set; }
        public string? Name { get; set; }
        public decimal Share { get; set; } = 0;
        public string? Type { get; set; }
    }
}
