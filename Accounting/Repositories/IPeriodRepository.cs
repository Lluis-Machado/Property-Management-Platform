using AccountingAPI.Models;

namespace AccountingAPI.Repositories
{
    public interface IPeriodRepository
    {
        Task<Period> InsertPeriodAsync(Period period);
        Task<IEnumerable<Period>> GetPeriodsAsync(Guid tenantId, bool includeDeleted = false);
        Task<Period> GetPeriodByIdAsync(Guid periodId);
        Task<Period> UpdatePeriodAsync(Period period);
        Task<int> SetDeletedPeriodAsync(Guid id, bool deleted);
    }
}
