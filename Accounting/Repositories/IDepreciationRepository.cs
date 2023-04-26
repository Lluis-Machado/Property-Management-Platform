using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IDepreciationRepository
    {
        Task<Guid> InsertDepreciationAsync(Depreciation Depreciation);
        Task<IEnumerable<Depreciation>> GetDepreciationsAsync();
        Task<Depreciation> GetDepreciationByIdAsync(Guid DepreciationId);
        Task<int> UpdateDepreciationAsync(Depreciation Depreciation);
    }
}
