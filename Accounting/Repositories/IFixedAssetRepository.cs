using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IFixedAssetRepository
    {
        Task<FixedAsset> InsertFixedAssetAsync(FixedAsset fixedAsset);
        Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync(bool includeDeleted = false);
        Task<FixedAsset?> GetFixedAssetByIdAsync(Guid fixedAssetId);
        Task<FixedAsset> UpdateFixedAssetAsync(FixedAsset fixedAsset);
        Task<int> SetDeletedFixedAssetAsync(Guid id, bool deleted);
    }
}
