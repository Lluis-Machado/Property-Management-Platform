namespace OwnershipAPI.DTOs
{
    public class OwnershipDetailedDto : OwnershipDto
    {
        public string OwnerName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
    }
}