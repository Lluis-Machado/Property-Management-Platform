using AutoMapper;
using CompanyAPI.Dtos;
using CompanyAPI.Models;
using CompanyAPI.Repositories;
using FluentValidation;
using MassTransit;
using MessagingContracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyAPI.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IValidator<CreateCompanyDto> _createCompanyValidator;
        private readonly IValidator<UpdateCompanyDto> _updateCompanyValidator;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public CompanyService(ICompanyRepository companyRepo,
            IValidator<CreateCompanyDto> createCompanyValidator,
            IValidator<UpdateCompanyDto> updateCompanyValidator,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _companyRepo = companyRepo;
            _createCompanyValidator = createCompanyValidator;
            _updateCompanyValidator = updateCompanyValidator;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ActionResult<CompanyDetailedDto>> CreateAsync(CreateCompanyDto createCompanyDto, string lastUser)
        {
            await _createCompanyValidator.ValidateAndThrowAsync(createCompanyDto);

            var company = _mapper.Map<CreateCompanyDto, Company>(TrimListItems(createCompanyDto));
            company.LastUpdateByUser = company.CreatedByUser = lastUser;
            company.CreatedAt = company.LastUpdateAt = DateTime.UtcNow;

            await _companyRepo.InsertOneAsync(company);
            await PerformAudit(company, company.Id);

            return new CreatedResult($"company/{company.Id}", _mapper.Map<Company, CompanyDetailedDto>(company));
        }

        private CreateCompanyDto TrimListItems(CreateCompanyDto createCompanyDto)
        {
            var emptyAddress = new CompanyAddress { AddressLine1 = "", AddressLine2 = "", City = "", PostalCode = "" };
            createCompanyDto.Addresses = createCompanyDto.Addresses
                .Where(address => !AreAddressesEqual(address, emptyAddress))
                .ToList();
            return createCompanyDto;
        }

        private bool AreAddressesEqual(CompanyAddress a1, CompanyAddress a2) => JsonSerializer.Serialize(a1) == JsonSerializer.Serialize(a2);

        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAsync(bool includeDeleted = false) => new OkObjectResult(await _companyRepo.GetAsync(includeDeleted));

        public async Task<CompanyDetailedDto> GetByIdAsync(Guid id) => _mapper.Map<Company, CompanyDetailedDto>(await _companyRepo.GetByIdAsync(id));

        public async Task<ActionResult<CompanyDetailedDto>> UpdateAsync(Guid companyId, UpdateCompanyDto updateCompanyDto, string lastUser)
        {
            await _updateCompanyValidator.ValidateAndThrowAsync(updateCompanyDto);

            var company = _mapper.Map<UpdateCompanyDto, Company>(updateCompanyDto);

            company.LastUpdateByUser = lastUser;
            company.LastUpdateAt = DateTime.UtcNow;
            company.Id = companyId;

            await _companyRepo.UpdateAsync(company);
            await PerformAudit(company, company.Id);

            return new OkObjectResult(_mapper.Map<Company, CompanyDetailedDto>(company));
        }

        public async Task<IActionResult> UpdateCompanyArchiveIdAsync(Guid companyId, Guid archiveId, string lastUser)
        {
            await _companyRepo.UpdateCompanyArchiveIdAsync(companyId, archiveId, lastUser);
            return new NoContentResult();
        }

        public async Task<IActionResult> SetDeleteStatusAsync(Guid companyId, bool isDeleted, string lastUser)
        {
            var updateResult = await _companyRepo.SetDeleteAsync(companyId, isDeleted, lastUser);
            return updateResult.IsAcknowledged ? new NoContentResult() : new NotFoundObjectResult("Company not found");
        }

        public Task<IActionResult> DeleteAsync(Guid companyId, string lastUser) => SetDeleteStatusAsync(companyId, true, lastUser);

        public Task<IActionResult> UndeleteAsync(Guid companyId, string lastUser) => SetDeleteStatusAsync(companyId, false, lastUser);

        public async Task PerformAudit(Company company, Guid id)
        {
            var audit = new Audit
            {
                Object = Serialize(company),
                Id = id,
                ObjectType = "Company"
            };

            await _publishEndpoint.Publish(new MessageContract { Payload = Serialize(audit) });
        }

        private string Serialize<T>(T obj) => JsonSerializer.Serialize(obj);
    }
}
