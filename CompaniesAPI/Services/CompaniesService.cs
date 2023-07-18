using AutoMapper;
using CompaniesAPI.Dtos;
using CompaniesAPI.Models;
using CompaniesAPI.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompaniesAPI.Services
{
    public class CompaniesService : ICompaniesService
    {
        private readonly ICompaniesRepository _companyRepo;
        private readonly IValidator<CreateCompanyDto> _createCompanyValidator;
        private readonly IValidator<UpdateCompanyDto> _updateCompanyValidator;
        private readonly IMapper _mapper;

        public CompaniesService(ICompaniesRepository companyRepo,
            IValidator<CreateCompanyDto> createCompanyValidator,
            IValidator<UpdateCompanyDto> updateCompanyValidator,
            IMapper mapper)
        {
            _companyRepo = companyRepo;
            _createCompanyValidator = createCompanyValidator;
            _updateCompanyValidator = updateCompanyValidator;
            _mapper = mapper;
        }

        public async Task<ActionResult<CompanyDetailedDto>> CreateAsync(CreateCompanyDto createCompanyDto, string lastUser)
        {
            var company = _mapper.Map<CreateCompanyDto, Company>(createCompanyDto);

            company.LastUpdateByUser = lastUser;
            company.CreatedByUser = lastUser;
            company.CreatedAt = DateTime.UtcNow;
            company.LastUpdateAt = DateTime.UtcNow;

            company = await _companyRepo.InsertOneAsync(company);

            var companyDto = _mapper.Map<Company, CompanyDetailedDto>(company);
            return new CreatedResult($"company/{companyDto.Id}", companyDto);
        }

        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAsync(bool includeDeleted = false)
        {
            var companies = await _companyRepo.GetAsync(includeDeleted);

            return new OkObjectResult(companies);
        }

        public async Task<CompanyDetailedDto> GetByIdAsync(Guid id)
        {
            var company = await _companyRepo.GetByIdAsync(id);

            var companyDTO = _mapper.Map<Company, CompanyDetailedDto>(company);

            return companyDTO;
        }

        public async Task<ActionResult<CompanyDetailedDto>> UpdateAsync(Guid companyId, UpdateCompanyDto updateCompanyDto, string lastUser)
        {
            var company = _mapper.Map<UpdateCompanyDto, Company>(updateCompanyDto);

            company.LastUpdateByUser = lastUser;
            company.LastUpdateAt = DateTime.UtcNow;
            company.Id = companyId;

            company = await _companyRepo.UpdateAsync(company);

            var companyDto = _mapper.Map<Company, CompanyDetailedDto>(company);

            return new OkObjectResult(companyDto);
        }

        public async Task<IActionResult> DeleteAsync(Guid companyId, string lastUser)
        {
            var updateResult = await _companyRepo.SetDeleteAsync(companyId, true, lastUser);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Company not found");

            return new NoContentResult();
        }

        public async Task<IActionResult> UndeleteAsync(Guid companyId, string lastUser)
        {
            var updateResult = await _companyRepo.SetDeleteAsync(companyId, false, lastUser);
            if (!updateResult.IsAcknowledged) return new NotFoundObjectResult("Company not found");

            return new NoContentResult();
        }
    }
}
