using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IDepreciationRepository
    {
        Task<Depreciation> InsertDepreciationAsync(Depreciation depreciation);
        Task<IEnumerable<Depreciation>> GetDepreciationsAsync(bool includeDeleted);
        Task<Depreciation> GetDepreciationByIdAsync(Guid depreciationId);
        Task<IEnumerable<Depreciation>> GetDepreciationByFAandPeriodAsync(Guid fixedAssetId, bool includeDeleted, DateTime? periodStart, DateTime? periodEnd);
        Task<int> UpdateDepreciationAsync(Depreciation depreciation);
        Task<int> SetDeleteDepreciationAsync(Guid id, bool deleted);
        Task<int> UpdateTotalDepreciationForFixedAsset(Guid fixedAssetId);
    }
}
