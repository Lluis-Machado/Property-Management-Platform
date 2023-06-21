using AccountingAPI.DTOs;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;

namespace AccountingAPI.Services
{
    public class BusinessPartnerService : IBusinessPartnerService
    {
        private readonly IBusinessPartnerRepository _businessPartnerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BusinessPartnerService> _logger;

        public BusinessPartnerService(IBusinessPartnerRepository businessPartnerRepository, ILogger<BusinessPartnerService> logger, IMapper mapper)
        {
            _businessPartnerRepository = businessPartnerRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<BusinessPartnerDTO> CreateBusinessPartnerAsync(CreateBusinessPartnerDTO createBusinessPartnerDTO, string? userName, Guid tenantId)
        {
            BusinessPartner businessPartner = _mapper.Map<BusinessPartner>(createBusinessPartnerDTO);
            businessPartner.CreatedBy = userName;
            businessPartner.LastModificationBy = userName;
            businessPartner.TenantId = tenantId;

            businessPartner = await _businessPartnerRepository.InsertBusinessPartnerAsync(businessPartner);
            return _mapper.Map<BusinessPartnerDTO>(businessPartner);
        }
        public async Task<IEnumerable<BusinessPartnerDTO>> GetBusinessPartnersAsync(bool includeDeleted = false)
        {
            IEnumerable<BusinessPartner> businessPartners = await _businessPartnerRepository.GetBusinessPartnersAsync(includeDeleted);
            return _mapper.Map<IEnumerable<BusinessPartnerDTO>>(businessPartners);
        }

        public async Task<BusinessPartnerDTO?> GetBusinessPartnerByIdAsync(Guid BusinessPartnerId)
        {
            BusinessPartner? businessPartner = await _businessPartnerRepository.GetBusinessPartnerByIdAsync(BusinessPartnerId);
            if (businessPartner == null) return null;
            return _mapper.Map<BusinessPartnerDTO>(businessPartner);
        }

        public async Task<bool> CheckIfBusinessPartnerExists(Guid BusinessPartnerId)
        {
            return await _businessPartnerRepository.GetBusinessPartnerByIdAsync(BusinessPartnerId) != null;
        }

        public async Task<BusinessPartnerDTO> UpdateBusinessPartnerAsync(CreateBusinessPartnerDTO createBusinessPartnerDTO, string? userName, Guid businessPartnerId)
        {
            BusinessPartner businessPartner = _mapper.Map<BusinessPartner>(createBusinessPartnerDTO);
            businessPartner.Id = businessPartnerId;
            businessPartner.LastModificationAt = DateTime.Now;
            businessPartner.LastModificationBy = userName;
            businessPartner = await _businessPartnerRepository.UpdateBusinessPartnerAsync(businessPartner);
            return _mapper.Map<BusinessPartnerDTO>(businessPartner);
        }

        public async Task<int> SetDeletedBusinessPartnerAsync(Guid businessPartnerId, bool deleted)
        {
            return await _businessPartnerRepository.SetDeletedBusinessPartnerAsync(businessPartnerId, deleted);
        }
    }
}