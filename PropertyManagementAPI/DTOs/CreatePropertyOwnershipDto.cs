namespace PropertiesAPI.DTOs;

public class CreatePropertyOwnershipDto
{
    public Guid Id { get; set; }
    public Guid ContactId { get; set; }
    public Guid PropertyId { get; set; }
    public bool MainOwnership { get; set; } = false;
    public decimal Share { get; set; } = 0;
}