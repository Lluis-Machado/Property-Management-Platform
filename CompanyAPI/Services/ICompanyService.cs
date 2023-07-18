using CompanyAPI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CompanyAPI.Services
{
    public interface ICompanyService
    {
        Task<ActionResult<CompanyDetailedDto>> CreateAsync(CreateCompanyDto createCompanyDto, string lastUser);
        Task<ActionResult<IEnumerable<CompanyDto>>> GetAsync(bool includeDeleted = false);
        Task<CompanyDetailedDto> GetByIdAsync(Guid id);
        Task<ActionResult<CompanyDetailedDto>> UpdateAsync(Guid companyId, UpdateCompanyDto updateCompanyDto, string lastUser);
        Task<IActionResult> DeleteAsync(Guid companyId, string lastUser);
        Task<IActionResult> UndeleteAsync(Guid companyId, string lastUser);
    }
}
