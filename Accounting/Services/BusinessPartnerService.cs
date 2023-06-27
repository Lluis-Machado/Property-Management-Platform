﻿using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class BusinessPartnerService : IBusinessPartnerService
    {
        private readonly IBusinessPartnerRepository _businessPartnerRepository;
        private readonly IValidator<CreateBusinessPartnerDTO> _createBusinessPartnerDTOValidator;
        private readonly IValidator<UpdateBusinessPartnerDTO> _updateBusinessPartnerDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessPartnerService> _logger;

        public BusinessPartnerService(IBusinessPartnerRepository businessPartnerRepository, IValidator<CreateBusinessPartnerDTO> createBusinessPartnerDTOValidator, IValidator<UpdateBusinessPartnerDTO> updateBusinessPartnerDTOValidator, ILogger<BusinessPartnerService> logger, IMapper mapper)
        {
            _businessPartnerRepository = businessPartnerRepository;
            _createBusinessPartnerDTOValidator = createBusinessPartnerDTOValidator;
            _updateBusinessPartnerDTOValidator = updateBusinessPartnerDTOValidator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BusinessPartnerDTO> CreateBusinessPartnerAsync(Guid tenantId, CreateBusinessPartnerDTO createBusinessPartnerDTO, string? userName)
        {
            // validation
            await _createBusinessPartnerDTOValidator.ValidateAndThrowAsync(createBusinessPartnerDTO);

            BusinessPartner businessPartner = _mapper.Map<BusinessPartner>(createBusinessPartnerDTO);
            businessPartner.CreatedBy = userName;
            businessPartner.LastModificationBy = userName;
            businessPartner.TenantId = tenantId;

            businessPartner = await _businessPartnerRepository.InsertBusinessPartnerAsync(businessPartner);
            return _mapper.Map<BusinessPartnerDTO>(businessPartner);
        }
        public async Task<IEnumerable<BusinessPartnerDTO>> GetBusinessPartnersAsync(Guid tenantId, bool includeDeleted = false)
        {
            IEnumerable<BusinessPartner> businessPartners = await _businessPartnerRepository.GetBusinessPartnersAsync(tenantId,includeDeleted);
            return _mapper.Map<IEnumerable<BusinessPartnerDTO>>(businessPartners);
        }

        public async Task<BusinessPartnerDTO> GetBusinessPartnerByIdAsync(Guid tenantId, Guid BusinessPartnerId)
        {
            BusinessPartner? businessPartner = await _businessPartnerRepository.GetBusinessPartnerByIdAsync(tenantId, BusinessPartnerId);
            
            if (businessPartner is null) throw new NotFoundException("Business Partner");
            
            return _mapper.Map<BusinessPartnerDTO>(businessPartner);
        }

        public async Task<BusinessPartnerDTO> UpdateBusinessPartnerAsync(Guid tenantId, Guid businessPartnerId, UpdateBusinessPartnerDTO updateBusinessPartnerDTO, string userName)
        {
            // validation
            await _updateBusinessPartnerDTOValidator.ValidateAndThrowAsync(updateBusinessPartnerDTO);

            // check if exists
            await GetBusinessPartnerByIdAsync(tenantId, businessPartnerId);

            BusinessPartner businessPartner = _mapper.Map<BusinessPartner>(updateBusinessPartnerDTO);
            businessPartner.Id = businessPartnerId;
            businessPartner.LastModificationAt = DateTime.Now;
            businessPartner.LastModificationBy = userName;
            businessPartner = await _businessPartnerRepository.UpdateBusinessPartnerAsync(businessPartner);

            return _mapper.Map<BusinessPartnerDTO>(businessPartner);
        }

        public async Task SetDeletedBusinessPartnerAsync(Guid tenantId, Guid businessPartnerId, bool deleted, string userName)
        {
            // check if exists
            await GetBusinessPartnerByIdAsync(tenantId, businessPartnerId);
            
            await _businessPartnerRepository.SetDeletedBusinessPartnerAsync(businessPartnerId, deleted, userName);
        }
    }
}