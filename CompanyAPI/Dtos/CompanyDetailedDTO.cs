using CompanyAPI.Models;

namespace CompanyAPI.Dtos;

public class CompanyDetailedDto
{
    public Guid Id { get; set; }
    // Addresses
    public List<CompanyAddress> Addresses { get; set; } = new List<CompanyAddress>();

}