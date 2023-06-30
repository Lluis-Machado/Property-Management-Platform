
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IDepreciationService
    {
        Task<IEnumerable<DepreciationDTO>> GetDepreciationsAsync(Guid tenantId);
        Task<IEnumerable<FixedAssetYearDetailsDTO>> GetFixedAssetsYearDetailsAsync(Guid tenantId, int year, bool includeDeleted = false, int? page = null, int? pageSize = null);
        Task<IEnumerable<DepreciationDTO>> GenerateDepreciationsAsync(Guid tenantId, Guid periodId, string userName);
    }
}
