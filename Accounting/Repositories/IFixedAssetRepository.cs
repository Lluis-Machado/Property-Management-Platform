using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IFixedAssetRepository
    {
        Task<FixedAsset> InsertFixedAssetAsync(FixedAsset fixedAsset);
        Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync();
        Task<FixedAsset> GetFixedAssetByIdAsync(Guid fixedAssetId);
        Task<int> UpdateFixedAssetAsync(FixedAsset fixedAsset);
        Task<int> SetDeleteFixedAssetAsync(Guid id, bool deleted);
    }
}
