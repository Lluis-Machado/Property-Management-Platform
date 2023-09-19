using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IFixedAssetService
    {
        Task<FixedAssetDTO> CreateFixedAssetAsync(Guid tenantId, CreateFixedAssetDTO createFixedAssetDTO, string userName);
        Task<IEnumerable<FixedAssetDTO>> GetFixedAssetsAsync(Guid tenantId, bool includeDeleted = false, int? page = null, int? pageSize = null);
        Task<FixedAssetDTO> GetFixedAssetByIdAsync(Guid tenantId, Guid FixedAssetId);
        Task<FixedAssetDTO> UpdateFixedAssetAsync(Guid tenantId, Guid fixedAssetId, UpdateFixedAssetDTO updateFixedAssetDTO, string userName);
        Task SetDeletedFixedAssetAsync(Guid tenantId, Guid fixedAssetId, bool deleted, string userName);
    }
}
