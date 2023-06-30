using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IDepreciationRepository
    {
        Task<Depreciation> InsertDepreciationAsync(Depreciation depreciation);
        Task<IEnumerable<Depreciation>> GetDepreciationsAsync(Guid tenantId, bool includeDeleted = false);
        Task<Depreciation> GetDepreciationByIdAsync(Guid tenantId, Guid depreciationId);
        Task<Depreciation> UpdateDepreciationAsync(Depreciation depreciation);
        Task<int> SetDeletedDepreciationAsync(Guid id, bool deleted);
    }
}
