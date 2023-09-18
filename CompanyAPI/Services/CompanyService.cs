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
            createCompanyDto = TrimListItems(createCompanyDto);

            var company = _mapper.Map<CreateCompanyDto, Company>(createCompanyDto);

            company.LastUpdateByUser = lastUser;
            company.CreatedByUser = lastUser;
            company.CreatedAt = DateTime.UtcNow;
            company.LastUpdateAt = DateTime.UtcNow;

            company = await _companyRepo.InsertOneAsync(company);
            _ = PerformAudit(company, company.Id);

            var companyDto = _mapper.Map<Company, CompanyDetailedDto>(company);
            return new CreatedResult($"company/{companyDto.Id}", companyDto);
        }

        private CreateCompanyDto TrimListItems(CreateCompanyDto createCompanyDto)
        {
            CompanyAddress emptyAddress = new();
            emptyAddress.AddressLine1 = "";
            emptyAddress.AddressLine2 = "";
            emptyAddress.ShortComment = null;
            emptyAddress.City = "";
            emptyAddress.PostalCode = "";

            List<CompanyAddress> emptyAddresses = new List<CompanyAddress>();

            foreach (var address in createCompanyDto.Addresses)
            {
                if (JsonSerializer.Serialize(address) != JsonSerializer.Serialize(emptyAddress))
                    emptyAddresses.Add(address);
            }
            createCompanyDto.Addresses = emptyAddresses;
            return createCompanyDto;

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
            await PerformAudit(company, company.Id);

            var companyDTO = _mapper.Map<Company, CompanyDetailedDto>(company);

            return new OkObjectResult(companyDTO);
        }

        public async Task<IActionResult> UpdateCompanyArchiveIdAsync(Guid companyId, Guid archiveId, string lastUser)
        {
            await _companyRepo.UpdateCompanyArchiveIdAsync(companyId, archiveId, lastUser);
            return new NoContentResult();
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

        public async Task PerformAudit(Company company, Guid id)
        {
            Audit audit = new Audit();
            audit.Object = JsonSerializer.Serialize(company);
            audit.Id = id;
            audit.ObjectType = "Company";

            string serializedAudit = JsonSerializer.Serialize(audit);
            MessageContract m = new MessageContract();
            m.Payload = serializedAudit;

            await _publishEndpoint.Publish<MessageContract>(m);
        }
    }
}
