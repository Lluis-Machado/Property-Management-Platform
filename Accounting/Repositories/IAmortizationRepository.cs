using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IAmortizationRepository
    {
        Task<Guid> InsertAmortizationAsync(Amortization amortization);
        Task<IEnumerable<Amortization>> GetAmortizationsAsync();
        Task<Amortization> GetAmortizationByIdAsync(Guid amortizationId);
        Task<int> UpdateAmortizationAsync(Amortization amortization);
    }
}
