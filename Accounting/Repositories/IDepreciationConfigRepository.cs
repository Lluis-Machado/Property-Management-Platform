using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IDepreciationConfigRepository
    {
        Task<Guid> InsertDepreciationConfigAsync(DepreciationConfig DepreciationConfig);
        Task<IEnumerable<DepreciationConfig>> GetDepreciationConfigsAsync();
        Task<DepreciationConfig> GetDepreciationConfigByIdAsync(Guid DepreciationConfigId);
        Task<int> UpdateDepreciationConfigAsync(DepreciationConfig DepreciationConfig);
    }
}
