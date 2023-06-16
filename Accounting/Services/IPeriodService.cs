using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IPeriodService
    {
        Task<PeriodDTO> CreatePeriodAsync(CreatePeriodDTO createPeriodDTO, string userName);
        Task<IEnumerable<PeriodDTO>> GetPeriodsAsync(bool includeDeleted = false);
        Task<PeriodDTO> GetPeriodByIdAsync(Guid PeriodId);
        Task<bool> CheckIfPeriodExistsAsync(Guid PeriodId);
        Task<PeriodDTO?> UpdatePeriodAsync(UpdatePeriodDTO updatePeriodDTO, string userName, Guid tenantId);
        Task<int> SetDeletedPeriodAsync(Guid tenantId, bool deleted);
    }
}
