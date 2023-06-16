
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IDepreciationService
    {
        Task<IEnumerable<DepreciationDTO>> GetDepreciationsAsync(Guid tenantId);
        Task<IEnumerable<FixedAssetYearDetailsDTO>> GetFixedAssetsYearDetailsAsync(Guid tenantId);
        Task<IEnumerable<DepreciationDTO>> SavePeriodDepreciationsAsync(Guid periodId, string userName);
    }
}
