using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IFixedAssetRepository
    {
        Task<Guid> InsertFixedAssetAsync(FixedAsset fixedAsset);
        Task<IEnumerable<FixedAsset>> GetFixedAssetsAsync();
        Task<FixedAsset> GetFixedAssetByIdAsync(Guid fixedAssetId);
        Task<int> UpdateFixedAssetAsync(FixedAsset fixedAsset);
    }
}
