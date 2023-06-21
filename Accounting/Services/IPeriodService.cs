using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IPeriodService
    {
        Task<PeriodDTO> CreatePeriodsAsync(CreatePeriodDTO createPeriodDTO, Guid TenantId, string? userName);
        Task<IEnumerable<PeriodDTO>> GetPeriodsAsync(Guid tenantId, bool includeDeleted = false);
        Task<PeriodDTO> GetPeriodByIdAsync(Guid PeriodId);
        Task<bool> CheckIfPeriodExistsByIdAsync(Guid periodId);
        Task<bool> CheckIfPeriodExistsAsync(Guid tenantId, int year, int month);
        Task<PeriodDTO?> UpdatePeriodStatusAsync(UpdatePeriodDTO.PeriodStatus status, string? userName, Guid periodId);
        Task<int> SetDeletedPeriodAsync(Guid tenantId, bool deleted);
    }
}
