using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IBusinessPartnerRepository
    {
        Task<BusinessPartner> InsertBusinessPartnerAsync(BusinessPartner businessPartner);
        Task<IEnumerable<BusinessPartner>> GetBusinessPartnersAsync(bool includeDeleted);
        Task<BusinessPartner?> GetBusinessPartnerByIdAsync(Guid businessPartnerId);
        Task<int> UpdateBusinessPartnerAsync(BusinessPartner businessPartner);
        Task<int> SetDeleteBusinessPartnerAsync(Guid id, bool deleted);
    }
}
