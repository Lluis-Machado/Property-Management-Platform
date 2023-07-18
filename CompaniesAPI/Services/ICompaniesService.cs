using CompaniesAPI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CompaniesAPI.Services
{
    public interface ICompaniesService
    {
        Task<ActionResult<CompanyDetailedDto>> CreateAsync(CreateCompanyDto createCompanyDto, string lastUser);
        Task<ActionResult<IEnumerable<CompanyDto>>> GetAsync(bool includeDeleted = false);
        Task<CompanyDetailedDto> GetByIdAsync(Guid id);
        Task<ActionResult<CompanyDetailedDto>> UpdateAsync(Guid companyId, UpdateCompanyDto updateCompanyDto, string lastUser);
        Task<IActionResult> DeleteAsync(Guid companyId, string lastUser);
        Task<IActionResult> UndeleteAsync(Guid companyId, string lastUser);
    }
}