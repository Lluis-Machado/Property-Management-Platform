using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IAmortizationCongifRepository
    {
        Task<Guid> InsertAmortizationConfigAsync(AmortizationConfig amortizationConfig);
        Task<IEnumerable<AmortizationConfig>> GetAmortizationConfigsAsync();
        Task<AmortizationConfig> GetAmortizationConfigByIdAsync(Guid amortizationConfigId);
        Task<int> UpdateAmortizationConfigAsync(AmortizationConfig amortizationConfig);
    }
}
