using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IBusinessPartnerService
    {
        Task<BusinessPartnerDTO> CreateBusinessPartnerAsync(CreateBusinessPartnerDTO createBusinessPartnerDTO, string? userName, Guid tenantId);
        Task<IEnumerable<BusinessPartnerDTO>> GetBusinessPartnersAsync(bool includeDeleted = false);
        Task<BusinessPartnerDTO?> GetBusinessPartnerByIdAsync(Guid businessPartnerId);
        Task<bool> CheckIfBusinessPartnerExists(Guid BusinessPartnerId);
        Task<BusinessPartnerDTO> UpdateBusinessPartnerAsync(CreateBusinessPartnerDTO createBusinessPartnerDTO, string? userName, Guid businessPartnerId);
        Task<int> SetDeletedBusinessPartnerAsync(Guid businessPartnerId, bool deleted);
    }
}
