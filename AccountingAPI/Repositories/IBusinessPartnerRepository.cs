using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IBusinessPartnerRepository
    {
        Task<BusinessPartner> InsertBusinessPartnerAsync(BusinessPartner businessPartner);
        Task<IEnumerable<BusinessPartner>> GetBusinessPartnersAsync(Guid? tenantId, bool includeDeleted = false);
        Task<IEnumerable<BusinessPartner>> GetBusinessPartnerByCIFAsync(string CIF, bool includeDeleted = false);
        Task<BusinessPartner?> GetBusinessPartnerByIdAsync(Guid tenantId, Guid businessPartnerId);
        Task<BusinessPartner> UpdateBusinessPartnerAsync(BusinessPartner businessPartner);
        Task<int> SetDeletedBusinessPartnerAsync(Guid id, bool deleted, string userName);
    }
}
