using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class FixedAssetService : IFixedAssetService
    {
        private readonly IFixedAssetRepository _fixedAssetRepository;
        private readonly IValidator<CreateFixedAssetDTO> _createFixedAssetDTOValidator;
        private readonly IValidator<UpdateFixedAssetDTO> _updateFixedAssetDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<APInvoiceService> _logger;

        public FixedAssetService(ILogger<APInvoiceService> logger, IMapper mapper, IFixedAssetRepository fixedAssetRepository, IValidator<CreateFixedAssetDTO> createFixedAssetDTOValidator, IValidator<UpdateFixedAssetDTO> updateFixedAssetDTOValidator)
        {
            _logger = logger;
            _mapper = mapper;
            _fixedAssetRepository = fixedAssetRepository;
            _createFixedAssetDTOValidator = createFixedAssetDTOValidator;
            _updateFixedAssetDTOValidator = updateFixedAssetDTOValidator;
        }

        public async Task<FixedAssetDTO> CreateFixedAssetAsync(Guid tenantId, CreateFixedAssetDTO createFixedAssetDTO, string userName)
        {
            // validation
            await _createFixedAssetDTOValidator.ValidateAndThrowAsync(createFixedAssetDTO);

            FixedAsset fixedAsset = _mapper.Map<FixedAsset>(createFixedAssetDTO);
            fixedAsset.CreatedBy = userName;
            fixedAsset.LastModificationBy = userName;

            fixedAsset = await _fixedAssetRepository.InsertFixedAssetAsync(fixedAsset);
            return _mapper.Map<FixedAssetDTO>(fixedAsset);
        }

        public async Task<IEnumerable<FixedAssetDTO>> GetFixedAssetsAsync(Guid tenantId, bool includeDeleted = false)
        {
            IEnumerable<FixedAsset> fixedAssets = await _fixedAssetRepository.GetFixedAssetsAsync(tenantId, true);
            return _mapper.Map<IEnumerable<FixedAssetDTO>>(fixedAssets);
        }

        public async Task<FixedAssetDTO> GetFixedAssetByIdAsync(Guid tenantId, Guid FixedAssetId)
        {
            FixedAsset? fixedAsset = await _fixedAssetRepository.GetFixedAssetByIdAsync(tenantId, FixedAssetId);

            if (fixedAsset is null) throw new NotFoundException("Fixed Asset");

            return _mapper.Map<FixedAssetDTO>(fixedAsset);
        }

        public async Task<FixedAssetDTO> UpdateFixedAssetAsync(Guid tenantId, Guid fixedAssetId, UpdateFixedAssetDTO updateFixedAssetDTO, string userName)
        {
            // validation
            await _updateFixedAssetDTOValidator.ValidateAndThrowAsync(updateFixedAssetDTO);

            // check if exists
            await GetFixedAssetByIdAsync(tenantId, fixedAssetId);

            FixedAsset fixedAsset = _mapper.Map<FixedAsset>(updateFixedAssetDTO);
            fixedAsset.Id = fixedAssetId;
            fixedAsset.LastModificationBy = userName;
            fixedAsset.LastModificationAt = DateTime.Now;
            fixedAsset = await _fixedAssetRepository.UpdateFixedAssetAsync(fixedAsset);

            return _mapper.Map<FixedAssetDTO>(fixedAsset);
        }

        public async Task SetDeletedFixedAssetAsync(Guid tenantId, Guid fixedAssetId, bool deleted, string userName)
        {
            // check if exists
            await GetFixedAssetByIdAsync(tenantId, fixedAssetId);

            await _fixedAssetRepository.SetDeletedFixedAssetAsync(tenantId, fixedAssetId, deleted, userName);
        }
    }
}