using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IBusinessPartnerService
    {
        Task<BusinessPartnerDTO> CreateBusinessPartnerAsync(Guid tenantId, CreateBusinessPartnerDTO createBusinessPartnerDTO, string userName);
        Task<IEnumerable<BusinessPartnerDTO>> GetBusinessPartnersAsync(Guid tenantId, bool includeDeleted = false);
        Task<BusinessPartnerDTO> GetBusinessPartnerByIdAsync(Guid tenantId, Guid businessPartnerId);
        Task<BusinessPartnerDTO> UpdateBusinessPartnerAsync(Guid tenantId, Guid businessPartnerId, UpdateBusinessPartnerDTO createBusinessPartnerDTO, string userName);
        Task SetDeletedBusinessPartnerAsync(Guid tenantId, Guid businessPartnerId, bool deleted, string userName);
    }
}
