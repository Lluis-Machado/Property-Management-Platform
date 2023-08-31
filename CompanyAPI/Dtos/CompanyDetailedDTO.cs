using CompanyAPI.Models;

namespace CompanyAPI.Dtos;

public class CompanyDetailedDto : CompanyDto
{
    public Guid Id { get; set; }
    // Addresses
    public List<CompanyAddress> Addresses { get; set; } = new();
    public List<CompanyContact> Contacts { get; set; } = new();
    public List<CompanyBankInformation> BankInformation { get; set; } = new();

}