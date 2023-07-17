namespace PropertiesAPI.Dtos
{
    public class OwnerDto
    {
        public Guid Id { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerType { get; set; } = string.Empty;
    }
}
