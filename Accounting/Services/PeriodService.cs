using AutoMapper;
using AccountingAPI.Repositories;
using AccountingAPI.Models;
using AccountingAPI.DTOs;

namespace AccountingAPI.Services
{
    public class PeriodService : IPeriodService
    {
        private readonly IPeriodRepository _periodRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PeriodService> _logger;

        public PeriodService(IPeriodRepository periodRepository, ILogger<PeriodService> logger, IMapper mapper)
        {
            _periodRepository = periodRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PeriodDTO> CreatePeriodAsync(CreatePeriodDTO createPeriodDTO, string userName)
        {
            Period period = _mapper.Map<Period>(createPeriodDTO);
            period.CreatedBy = userName;
            period.LastModificationBy = userName;

            period = await _periodRepository.InsertPeriodAsync(period);
            return _mapper.Map<PeriodDTO>(period);  
        }
        public async Task<IEnumerable<PeriodDTO>> GetPeriodsAsync(bool includeDeleted = false)
        {
            IEnumerable<Period> periods = await _periodRepository.GetPeriodsAsync(includeDeleted);
            return _mapper.Map<IEnumerable<Period>, List<PeriodDTO>>(periods);
        }

        public async Task<PeriodDTO> GetPeriodByIdAsync(Guid periodId)
        {
            Period period = await _periodRepository.GetPeriodByIdAsync(periodId);
            return _mapper.Map<PeriodDTO>(period);
        }

        public async Task<bool> CheckIfPeriodExistsAsync(Guid periodId)
        {
            return await _periodRepository.GetPeriodByIdAsync(periodId) != null;
        }

        public async Task<PeriodDTO?> UpdatePeriodAsync(UpdatePeriodDTO updatePeriodDTO, string userName, Guid periodId)
        {
            Period period = _mapper.Map<Period>(updatePeriodDTO);
            period.Id = periodId;
            period.LastModificationAt = DateTime.Now;
            period.LastModificationBy = userName;

            period = await _periodRepository.UpdatePeriodAsync(period);
           return _mapper.Map<PeriodDTO>(period);
        }

        public async Task<int> SetDeletedPeriodAsync(Guid periodId, bool deleted)
        {
            return await _periodRepository.SetDeletedPeriodAsync(periodId,deleted);
        }
    }
}