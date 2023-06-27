using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant> InsertTenantAsync(Tenant tenant);
        Task<IEnumerable<Tenant>> GetTenantsAsync(bool includeDeleted = false);
        Task<Tenant> GetTenantByIdAsync(Guid tenantId);
        Task<Tenant> UpdateTenantAsync(Tenant tenant);
        Task<int> SetDeletedTenantAsync(Guid id, bool deleted, string userName);
    }
}
