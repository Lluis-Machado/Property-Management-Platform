namespace CompaniesAPI.Dtos;

public class CompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string? Nif { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

}