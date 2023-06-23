using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IBusinessPartnerRepository
    {
        Task<BusinessPartner> InsertBusinessPartnerAsync(BusinessPartner businessPartner);
        Task<IEnumerable<BusinessPartner>> GetBusinessPartnersAsync(bool includeDeleted = false);
        Task<BusinessPartner?> GetBusinessPartnerByIdAsync(Guid businessPartnerId);
        Task<BusinessPartner> UpdateBusinessPartnerAsync(BusinessPartner businessPartner);
        Task<int> SetDeletedBusinessPartnerAsync(Guid id, bool deleted);
    }
}
