using AccountingAPI.DTOs;
using AccountingAPI.Exceptions;
using AccountingAPI.Models;
using AccountingAPI.Repositories;
using AutoMapper;
using FluentValidation;

namespace AccountingAPI.Services
{
    public class PeriodService : IPeriodService
    {
        private readonly IPeriodRepository _periodRepository;
        private readonly IValidator<CreatePeriodDTO> _createPeriodDTOValidator;
        private readonly IValidator<UpdatePeriodDTO> _updatePeriodDTOValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<PeriodService> _logger;

        public PeriodService(IPeriodRepository periodRepository, ILogger<PeriodService> logger, IMapper mapper, IValidator<CreatePeriodDTO> createPeriodDTOValidator, IValidator<UpdatePeriodDTO> updatePeriodDTOValidator)
        {
            _periodRepository = periodRepository;
            _createPeriodDTOValidator = createPeriodDTOValidator;
            _updatePeriodDTOValidator = updatePeriodDTOValidator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PeriodDTO> CreatePeriodAsync(Guid tenantId, CreatePeriodDTO createPeriodDTO, string userName)
        {
            // validation
            await _createPeriodDTOValidator.ValidateAndThrowAsync(createPeriodDTO);

            IEnumerable<PeriodDTO> periodDTOs = await GetPeriodsAsync(tenantId);

            bool exists = periodDTOs.Any(p => p.Year == createPeriodDTO.Year && p.Month == createPeriodDTO.Month);

            if (exists) throw new ConflictException("Period already exists");

            Period period = _mapper.Map<Period>(createPeriodDTO);
            period.TenantId = tenantId;
            period.CreatedBy = userName;
            period.LastModificationBy = userName;

            period = await _periodRepository.InsertPeriodAsync(period);

            return _mapper.Map<PeriodDTO>(period);
        }

        public async Task<IEnumerable<PeriodDTO>> GetPeriodsAsync(Guid tenantId, bool includeDeleted = false)
        {
            IEnumerable<Period> periods = await _periodRepository.GetPeriodsAsync(tenantId, includeDeleted);
            return _mapper.Map<IEnumerable<Period>, List<PeriodDTO>>(periods);
        }

        public async Task<PeriodDTO> GetPeriodByIdAsync(Guid tenantId, Guid periodId)
        {
            Period? period = await _periodRepository.GetPeriodByIdAsync(tenantId, periodId);

            if (period is null) throw new NotFoundException("Period");

            return _mapper.Map<PeriodDTO>(period);
        }

        public async Task<PeriodDTO> UpdatePeriodStatusAsync(Guid tenantId, Guid periodId, UpdatePeriodDTO updatePeriodDTO, string userName)
        {
            // validation
            await _updatePeriodDTOValidator.ValidateAndThrowAsync(updatePeriodDTO);

            // check if exists
            await GetPeriodByIdAsync(tenantId, periodId);

            Period period = _mapper.Map<Period>(updatePeriodDTO);
            period.Id = periodId;
            period.LastModificationAt = DateTime.Now;
            period.LastModificationBy = userName;

            period = await _periodRepository.UpdatePeriodAsync(period);

            return _mapper.Map<PeriodDTO>(period);
        }

        public async Task SetDeletedPeriodAsync(Guid tenantId, Guid periodId, bool deleted, string userName)
        {
            // check if exists
            PeriodDTO periodDTO = await GetPeriodByIdAsync(tenantId, periodId);

            // check if already deleted/undeleted
            if (periodDTO.Deleted == deleted)
            {
                string action = deleted ? "deleted" : "undeleted";
                throw new ConflictException($"Period already {action}");
            }

            await _periodRepository.SetDeletedPeriodAsync(periodId, deleted, userName);
        }
    }
}