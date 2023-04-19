using Accounting.Models;

namespace Accounting.Repositories
{
    public interface ITenantRepository
    {
        Task<Guid> InsertTenantAsync(Tenant tenant);
        Task<IEnumerable<Tenant>> GetTenantsAsync();
        Task<Tenant> GetTenantByIdAsync(Guid tenantId);
        Task<int> UpdateTenantAsync(Tenant tenant);
    }
}
