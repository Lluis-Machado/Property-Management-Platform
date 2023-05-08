using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IDepreciationConfigRepository
    {
        Task<DepreciationConfig> InsertDepreciationConfigAsync(DepreciationConfig DepreciationConfig);
        Task<IEnumerable<DepreciationConfig>> GetDepreciationConfigsAsync();
        Task<DepreciationConfig> GetDepreciationConfigByIdAsync(Guid DepreciationConfigId);
        Task<int> UpdateDepreciationConfigAsync(DepreciationConfig DepreciationConfig);
        Task<int> SetDeleteDepreciationConfigAsync(Guid id, bool deleted);
    }
}
