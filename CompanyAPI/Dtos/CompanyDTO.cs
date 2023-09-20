namespace CompanyAPI.Dtos;

public class CompanyDto
{
    // Company Information
    public string Name { get; set; } = string.Empty;
    public string? Nif { get; set; }
    public string? GermanTaxOffice { get; set; }
    public string? CompanyPurpose { get; set; }
    public string? UStIDNumber { get; set; }
    public DateOnly? FoundingDate { get; set; }


    // Contact Information
    public string? Email { get; set; }
    public int? CountryMaskId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Comments { get; set; }

    public Guid? TenantId { get; set; }
    public Guid? ArchiveId { get; set; }


}