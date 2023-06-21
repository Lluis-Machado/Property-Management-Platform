using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public interface IFixedAssetService
    {
        Task<FixedAssetDTO> CreateFixedAssetAsync(CreateFixedAssetDTO createFixedAssetDTO, string? userName);
        Task<IEnumerable<FixedAssetDTO>> GetFixedAssetsAsync();
        Task<FixedAssetDTO?> GetFixedAssetByIdAsync(Guid FixedAssetId);
        Task<FixedAssetDTO> UpdateFixedAssetAsync(UpdateFixedAssetDTO updateFixedAssetDTO, string? userName, Guid fixedAssetId);
        Task<int> SetDeletedFixedAssetAsync(Guid fixedAssetId, bool deleted);
    }
}
