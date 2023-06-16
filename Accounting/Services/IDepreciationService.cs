
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IDepreciationService
    {
        Task<IEnumerable<DepreciationDTO>> GetDepreciationsAsync();
        //Task<IEnumerable<DepreciationDTO>> SavePeriodDepreciationsAsync(Guid periodId, string userName);
    }
}
