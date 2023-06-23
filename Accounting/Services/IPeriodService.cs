using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IPeriodService
    {
        Task<PeriodDTO> CreatePeriodAsync(CreatePeriodDTO createPeriodDTO, Guid TenantId, string userName);
        Task<IEnumerable<PeriodDTO>> GetPeriodsAsync(Guid tenantId, bool includeDeleted = false);
        Task<PeriodDTO> GetPeriodByIdAsync(Guid tenantId, Guid periodId);
        Task<PeriodDTO> UpdatePeriodStatusAsync(Guid tenantId, Guid periodId,UpdatePeriodDTO updatePeriodDTO, string userName);
        Task<int> SetDeletedPeriodAsync(Guid tenantId,Guid period, bool deleted);
    }
}
