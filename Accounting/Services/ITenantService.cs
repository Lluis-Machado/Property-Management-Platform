using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface ITenantService
    {
        Task<TenantDTO> CreateTenantAsync(CreateTenantDTO createTenantDTO, string userName);
        Task<IEnumerable<TenantDTO>> GetTenantsAsync(bool includeDeleted = false);
        Task<TenantDTO> GetTenantByIdAsync(Guid TenantId);
        Task<TenantDTO> UpdateTenantAsync(Guid tenantId, UpdateTenantDTO updateTenantDTO, string userName);
        Task SetDeletedTenantAsync(Guid tenantId, bool deleted, string userName);
    }
}
