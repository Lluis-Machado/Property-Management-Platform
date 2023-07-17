using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IFixedAssetRepository
    {
        Task<FixedAsset> InsertFixedAssetAsync(FixedAsset fixedAsset);
        Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync(Guid tenantId, bool includeDeleted = false);
        Task<FixedAsset?> GetFixedAssetByIdAsync(Guid tenantId, Guid fixedAssetId);
        Task<FixedAsset> UpdateFixedAssetAsync(FixedAsset fixedAsset);
        Task<int> SetDeletedFixedAssetAsync(Guid tenantId, Guid fixedAssetId, bool deleted, string userName);
    }
}
