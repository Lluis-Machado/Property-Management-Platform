using AccountingAPI.Repositories;
using AccountingAPI.Models;
using System.Transactions;
using AutoMapper;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public class FixedAssetService : IFixedAssetService
    {
        private readonly IFixedAssetRepository _fixedAssetRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<APInvoiceService> _logger;

        public FixedAssetService(ILogger<APInvoiceService> logger, IMapper mapper, IFixedAssetRepository fixedAssetRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _fixedAssetRepository = fixedAssetRepository;
        }

        public async Task<FixedAssetDTO> CreateFixedAssetAsync(CreateFixedAssetDTO createFixedAssetDTO, string userName)
        {
            FixedAsset fixedAsset = _mapper.Map<FixedAsset>(createFixedAssetDTO);
            fixedAsset.CreatedBy = userName;
            fixedAsset.LastModificationBy = userName;
            fixedAsset = await _fixedAssetRepository.InsertFixedAssetAsync(fixedAsset);
            return _mapper.Map<FixedAssetDTO>(fixedAsset);
        }

        public async Task<IEnumerable<FixedAssetDTO>> GetFixedAssetsAsync()
        {
            IEnumerable<FixedAsset> fixedAssets = await _fixedAssetRepository.GetFixedAssetsAsync(true);
            return _mapper.Map<IEnumerable<FixedAssetDTO>> (fixedAssets);
        }

        public async Task<FixedAssetDTO?> GetFixedAssetByIdAsync(Guid FixedAssetId)
        {
            FixedAsset? fixedAsset = await _fixedAssetRepository.GetFixedAssetByIdAsync(FixedAssetId);
            return _mapper.Map<FixedAssetDTO>(fixedAsset);
        }

        public async Task<FixedAssetDTO> UpdateFixedAssetAsync(UpdateFixedAssetDTO updateFixedAssetDTO, string userName, Guid fixedAssetId)
        {
            FixedAsset fixedAsset = _mapper.Map<FixedAsset>(updateFixedAssetDTO);
            fixedAsset.Id = fixedAssetId;
            fixedAsset.LastModificationBy = userName;
            fixedAsset.LastModificationAt = DateTime.Now;
            fixedAsset = await _fixedAssetRepository.UpdateFixedAssetAsync(fixedAsset);
            return _mapper.Map<FixedAssetDTO>(fixedAsset);
        }

        public async Task<int> SetDeletedFixedAssetAsync(Guid fixedAssetId, bool deleted)
        {
            return await _fixedAssetRepository.SetDeletedFixedAssetAsync(fixedAssetId, deleted);
        }
    }
}