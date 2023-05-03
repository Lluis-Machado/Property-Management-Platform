﻿using Accounting.Models;

namespace Accounting.Repositories
{
    public interface IDepreciationRepository
    {
        Task<Depreciation> InsertDepreciationAsync(Depreciation depreciation);
        Task<IEnumerable<Depreciation>> GetDepreciationsAsync();
        Task<Depreciation> GetDepreciationByIdAsync(Guid depreciationId);
        Task<int> UpdateDepreciationAsync(Depreciation depreciation);
        Task<int> SetDeleteDepreciationAsync(Guid id, bool deleted);
    }
}
